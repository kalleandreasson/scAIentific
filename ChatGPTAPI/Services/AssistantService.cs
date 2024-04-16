using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Threads;

public class AssistantService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly OpenAIClient _assistantApi;
    private readonly MongoDBService _mongoDBService;
    private readonly ILogger<AssistantService> _logger;


    public AssistantService(HttpClient httpClient, IOptions<OpenAIServiceOptions> options, MongoDBService mongoDBService, ILogger<AssistantService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _assistantApi = new OpenAIClient(_apiKey);
        _mongoDBService = mongoDBService;
        _logger = logger;
    }

    //Save fileID, assistantID, threadID to database
    //Should only returns assistantID
    //Create a new service for file controller (refactoring)
   public async Task<AssistantObj> CreateAssistantWithFileUploadAndThread(string filePath, string researchArea, string userName)
    {
        try
        {
            var userAssistantObj = await _mongoDBService.GetAssistantObjIfExsistAsync(userName);
            if (userAssistantObj != null)
            {
                _logger.LogInformation($"the user: {userName} already has an assistant with id:{userAssistantObj.AssistantID}.");

                DeleteFileFromServer(filePath);

                return userAssistantObj;
            };
            var createdAssistant = await CreateNewAssistant(researchArea);
            var createdFileId = await UploadFileToAssistant(filePath, createdAssistant);

            DeleteFileFromServer(filePath);

            var CreatedThreadId = await CreateThread();
            _logger.LogInformation($"No existing assistant found. Creating new assistant for user: {userName}.");
            var newUserAssistantObj = new AssistantObj
            {
                Username = userName,
                AssistantID = createdAssistant,
                FileID = createdFileId,
                ThreadID = CreatedThreadId
            };
            await _mongoDBService.SaveAssistantAsync(newUserAssistantObj);
            return newUserAssistantObj;

        }
        catch (Exception ex)
        {
            _logger.LogError($"An error occurred while creating or updating the assistant for {userName}: {ex}");
            return null;
        }
    }

    private async Task<AssistantResponse> CreateNewAssistant(string researchArea)
    {
        try
        {
            var tools = new List<Tool> { Tool.Retrieval };

            var request = new CreateAssistantRequest("gpt-4-turbo-preview", "Research expert", null, $"You have demonstrated proficiency in analyzing abstracts of research articles to identify and find the research articles forefront in \"{researchArea}\", Please review the information provided in the attached file. Based on your analysis, formulate a comprehensive response.", tools);

            var createdAssistant = await _assistantApi.AssistantsEndpoint.CreateAssistantAsync(request);

            _logger.LogInformation($"Successfully created Assistant: {createdAssistant.Id}");
            return createdAssistant;
        }
        catch (Exception ex)
        {

            _logger.LogError(ex, $"Failed to create assistant for research area '{researchArea}'.");
            throw;
        }

    }

    private async Task<string> UploadFileToAssistant(string filePath, string assistantId)
    {
        _logger.LogInformation($"Starting file upload process for assistant ID: {assistantId}.");

        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantId);
        if (assistant == null)
        {
            throw new InvalidOperationException($"Assistant with ID {assistantId} not found.");
        }

        _logger.LogInformation("Uploading new file...");
        var assistantFile = await assistant.UploadFileAsync(filePath);
        if (assistantFile == null || string.IsNullOrWhiteSpace(assistantFile.Id))
        {
            _logger.LogError("Failed to upload the new file.");
            throw new InvalidOperationException("Failed to upload the new file.");
        }

        _logger.LogInformation($"New file uploaded successfully: {assistantFile.Id}");
        return assistantFile.Id;
    }

    private void DeleteFileFromServer(string filePath)
    {
        try
        {
            File.Delete(filePath);
        }
        catch (IOException ioEx)
        {
            _logger.LogError($"IO exception occurred while deleting file '{filePath}' from the server: {ioEx.Message}");
        }
    }


    private async Task<string> CreateThread()
    {
        try
        {
            _logger.LogInformation("Creating a new thread.");
            var thread = await _assistantApi.ThreadsEndpoint.CreateThreadAsync();

            if (thread == null || string.IsNullOrEmpty(thread.Id))
            {
                _logger.LogError("Failed to create a new thread.");
                throw new InvalidOperationException("Thread creation failed due to a null response.");
            }

            _logger.LogInformation($"New thread created with ID: {thread.Id} at {thread.CreatedAt}.");
            return thread.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occurred while creating a new thread: {ex.Message}");
            throw; // Rethrow the exception to allow upstream handling.
        }
    }


    public async Task<string> DeleteUserAssistantAndThreadsFromApiAndDB(string userName)
    {
        try
        {
            var userAssistantObj = await _mongoDBService.GetAssistantObjIfExsistAsync(userName);
            if (userAssistantObj == null)
            {
                throw new KeyNotFoundException($"User '{userName}' or their assistant does not exist.");
            }
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(userAssistantObj.AssistantID);
            if (assistant == null)
            {
                throw new InvalidOperationException($"Assistant with ID {userAssistantObj.AssistantID} not found.");
            }

            _logger.LogInformation($"Deleting assistant and its file for user: {userName}.");
            // Perform deletions in parallel to improve efficiency
            var deleteFileTask = await _assistantApi.FilesEndpoint.DeleteFileAsync(userAssistantObj.FileID);
            var deleteAssistantTask = await _assistantApi.AssistantsEndpoint.DeleteAssistantAsync(assistant);
            var deleteThreadTask = await _assistantApi.ThreadsEndpoint.DeleteThreadAsync(userAssistantObj.ThreadID);

            // await Task.WhenAll(deleteFileTask, deleteAssistantTask, deleteThreadTask);

            await _mongoDBService.DeleteUserAssistantDetailsAsync(userName);

            _logger.LogInformation($"Successfully deleted all resources for user: {userName}.");
            return $"Successfully deleted assistant and resources for user '{userName}'.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while deleting resources for user '{userName}'.");
            return $"An error occurred while deleting resources for user '{userName}': {ex.Message}";
        }
    }

    public async Task<string> DeleteAllAssistantsAndThreadsAsync()
    {
        try
        {
            var assistantsList = await _assistantApi.AssistantsEndpoint.ListAssistantsAsync().ConfigureAwait(false);
            _logger.LogInformation("Starting deletion of all assistants and their files.");

            foreach (var assistant in assistantsList.Items)
            {
                var remainingFiles = await DeleteAssistantFiles(assistant.Id).ConfigureAwait(false);
                var isDeleted = await _assistantApi.AssistantsEndpoint.DeleteAssistantAsync(assistant).ConfigureAwait(false);

                if (isDeleted)
                {
                    _logger.LogInformation($"Successfully deleted assistant {assistant.Id} and their files. Remaining files: {remainingFiles}.");
                }
                else
                {
                    _logger.LogWarning($"Failed to delete assistant {assistant.Id}.");
                }
            }

            _logger.LogInformation("Starting deletion of all threads.");
            var threadIDList = await _mongoDBService.GetAllThreadIDsAsync().ConfigureAwait(false);
            foreach (var threadID in threadIDList)
            {
                var isDeleted = await _assistantApi.ThreadsEndpoint.DeleteThreadAsync(threadID).ConfigureAwait(false);
                _logger.LogInformation($"Thread {threadID} deletion status: {isDeleted}.");
            }

            await _mongoDBService.DeleteAllAssistantsAsync().ConfigureAwait(false);
            _logger.LogInformation("Completed deletion of all assistants, their files, and threads.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while deleting assistants and threads.");
            return $"An error occurred: {ex.Message}";
        }

        return "All assistants and threads deleted successfully.";
    }

    public async Task<int> DeleteAssistantFiles(string assistantID)
    {
        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);

        var filesList = await assistant.ListFilesAsync();
        foreach (var file in filesList.Items)
        {
            await assistant.DeleteFileAsync(file.Id);
        }

        var filesListAfterDeletion = await assistant.ListFilesAsync();

        return filesListAfterDeletion.Items.Count;
    }


    /// <summary>
    /// this method check if the user has and assistant and if the user has an assistant it check if it exist in the api.
    /// </summary>
    /// <returns></returns> <summary>
    /// 
    /// </summary>
    /// <returns>assistantId </returns>
    /// <summary>
    /// Checks if the user has an assistant and verifies its existence in the API.
    /// </summary>
    /// <param name="userName">The user name to check for an associated assistant.</param>
    /// <returns>The Assistant ID if found, otherwise a message indicating no assistant is found.</returns>
    public async Task<string> GetUserAssistantAsync(string userName)
    {
        try
        {
            var userAssistantObj = await _mongoDBService.GetAssistantObjIfExsistAsync(userName);
            if (userAssistantObj == null || string.IsNullOrEmpty(userAssistantObj.AssistantID))
            {
                _logger.LogInformation($"No assistant found for user {userName}.");
                return "No assistant is found";
            }
            return userAssistantObj.AssistantID;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving assistant for user {userName}.");
            throw;
        }
    }



    public async Task<ListResponse<AssistantResponse>> ListAllApisAssistant()
    {
        try
        {
            var assistantsList = await _assistantApi.AssistantsEndpoint.ListAssistantsAsync();
            if (!assistantsList.Items.Any())
            {
                _logger.LogInformation("No assistants found.");
            }
            else
            {
                _logger.LogInformation($"Found {assistantsList.Items.Count} assistant(s).");
                // Optionally log details for each assistant
                foreach (var assistant in assistantsList.Items)
                {
                    LogAssistantDetails(assistant);
                }
            }

            return assistantsList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while retrieving all assistants.");
            throw;
        }
    }



    private void LogAssistantDetails(AssistantResponse assistant)
    {
        var assistantDetails = new StringBuilder()
            .AppendLine($"ID: {assistant.Id}")
            .AppendLine($"Name: {assistant.Name}")
            .AppendLine($"Model: {assistant.Model}")
            .AppendLine($"CreatedAt: {assistant.CreatedAt.ToString()}") // Ensure ToString() if needed
            .AppendLine($"Number of files: {assistant.FileIds.Count}");

        foreach (var fileId in assistant.FileIds)
        {
            assistantDetails.AppendLine($"FileId: {fileId}");
        }

        // Check if Tools is not null and iterate
        if (assistant.Tools != null)
        {
            foreach (var tool in assistant.Tools)
            {
                assistantDetails.AppendLine($"Tool: {tool.Type}");
            }
        }

        assistantDetails.AppendLine("-------------");
        _logger.LogInformation(assistantDetails.ToString());
    }

}
