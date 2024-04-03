using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Threads;

public class FileManagerService
{
    private readonly MongoDBService _mongoDBService;
    private readonly HttpClient _httpClient;
    private readonly OpenAIClient _assistantApi;
    private readonly string _apiKey;
    private readonly ILogger<FileManagerService> _logger;
    public FileManagerService(ILogger<FileManagerService> logger, HttpClient httpClient, IOptions<OpenAIServiceOptions> options, MongoDBService mongoDBService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _assistantApi = new OpenAIClient(_apiKey);
        _mongoDBService = mongoDBService;
        _logger = logger;
    }

    public async Task<string> ReplaceAssistantFile(string userName, string filePath)
    {
        // ToDO: Add the userId to the userObject in the database and use it instead of username
        // ToDo: modify the file saver to save the file by the userId
        // ToDo: code a method that will find user file by the userID in the wwwroot and return the filePath

        try
        {
            // First, attempt to delete the old file from the assistant.
            bool isDeleted = await DeleteAssistantFile(userName);
            if (!isDeleted)
            {
                // If the file couldn't be deleted, stop the process and indicate failure.
                throw new InvalidOperationException("Failed to delete the old file.");
            }

            // Next, try to upload the new file to the assistant.
            string newAssistantFileId = await UploadFileToAssistantAsync(userName, filePath);
            if (string.IsNullOrWhiteSpace(newAssistantFileId))
            {
                // If the file couldn't be uploaded, stop the process and indicate failure.
                throw new InvalidOperationException("Failed to upload the new file to the assistant.");
            }

            // Only if both prior operations succeeded, update the database reference.
            await UpdateDatabaseFileReference(userName, newAssistantFileId);

            // Optionally, delete the uploaded file from the wwwroot folder here if necessary.

            return newAssistantFileId;
        }
        catch (Exception ex)
        {
            // Log the exception
            // Consider how to handle the exception (e.g., retry logic, return a specific error code, etc.)
            throw; // Rethrowing the exception for now; adjust based on your error handling strategy
        }
    }

    private async Task<string> UploadFileToAssistantAsync(string userName, string filePath)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("Invalid arguments provided for uploading the file.");
            }

            var user = await _mongoDBService.GetUserIfExistsAsync(userName);
            if (user == null)
            {
                throw new InvalidOperationException($"User '{userName}' does not exist.");
            }

            // Example for specific exception handling
            try
            {
                string assistantId = user.AssistantID;
                var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantId);
                var assistantFile = await assistant.UploadFileAsync(filePath);

                if (assistantFile == null || string.IsNullOrWhiteSpace(assistantFile.Id))
                {
                    throw new InvalidOperationException("Failed to upload the new file.");
                }

                return assistantFile?.Id;
            }
            catch (HttpRequestException httpEx)
            {
                // Catch specific exceptions related to API operations
                // Log with context
                _logger.LogError($"API exception occurred while uploading file for user '{userName}': {httpEx.Message}");
                throw; // Optionally, add more context or wrap the exception in a custom exception
            }
        }
        catch (ArgumentException argEx)
        {
            _logger.LogError($"Argument exception in UploadFileToAssistantAsync: {argEx.Message}");
            throw; // Rethrow if you can't handle it here, but log it for debugging purposes
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in UploadFileToAssistantAsync for user '{userName}': {ex.Message}");
            throw; // Rethrow with generic or specific handling as needed
        }
    }


    // Gets the userFileId from the database and deletes it from the assistant in the api.
    private async Task<bool> DeleteAssistantFile(string userName)
    {
        try
        {
            var user = await _mongoDBService.GetUserIfExistsAsync(userName);
            if (user == null)
            {
                _logger.LogWarning($"Attempted to delete a file for a non-existent user: {userName}");
                return false; // Or throw a more specific exception if appropriate
            }
            string assistantId = user.AssistantID;
            string userFileId = user.FileID; // Use await instead of .Result
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantId);

            bool isDeleted = await assistant.DeleteFileAsync(userFileId);
            _logger.LogInformation($"Delete operation status for {userName}: {isDeleted}");
            return isDeleted;
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, $"HTTP request exception occurred while deleting file for user '{userName}'.");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Unexpected error in DeleteAssistantFile for user '{userName}': {ex.Message}");
            throw; // Rethrowing here, but with logging for diagnostics
        }
    }



    private async Task UpdateDatabaseFileReference(string userName, string newFileId)
    {
        await _mongoDBService.ReplaceFileIdForUserAsync(userName, newFileId);
    }



    public async Task<int> ListAssistantFiles(string userName)
    {
        Console.WriteLine("list service");
        var user = await _mongoDBService.GetUserIfExistsAsync(userName);
        if (user == null)
        {
            _logger.LogWarning($"User '{userName}' not found.");
            return 0; // or throw a more specific exception
        }
        string assistantID = user.AssistantID;

        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);

        var filesList = await assistant.ListFilesAsync();
        foreach (var file in filesList.Items)
        {

            Console.WriteLine($"{file.AssistantId}'s file -> {file.Id}");
        }

        return filesList.Items.Count;
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
}