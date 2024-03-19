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
    private readonly string _threadId;
    private readonly string _assistantId;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly OpenAIClient _assistantApi;

    private string _researchArea = " Gender and Social Networks in Organizational Setting";

    public AssistantService(HttpClient httpClient, IOptions<OpenAIServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _assistantApi = new OpenAIClient(_apiKey);
    }

    public async Task<AssistantObj> CreateAssistant(string filePath)
    {
        var flag = await deleteAssistant();
        Console.WriteLine(flag);
        var tools = new List<Tool> { Tool.Retrieval };

        Console.WriteLine("inside Assistant method");
        try
        {
            var assistantID = await checkAssistant();
            if (assistantID == "No assistants found")
            {
                var request = new CreateAssistantRequest("gpt-4-turbo-preview", "Research expert", null, $"You have demonstrated proficiency in analyzing abstracts of research articles to identify and find the research articles forefront in \"{_researchArea}\", Please review the information provided in the attached file. Based on your analysis, formulate a comprehensive response.", tools);
                var assistantCreate = await _assistantApi.AssistantsEndpoint.CreateAssistantAsync(request);
                Console.WriteLine($"Created Assistant: {assistantCreate}");
                var fileID = await UploadFileAsync(filePath, assistantCreate);
                var threadId = await CreateThread();

                var newAssistantObj = new AssistantObj
                {
                    AssistantID = assistantCreate,
                    ThreadID = threadId,
                    Username = "singletonUser"
                    // Initialize other fields as necessary
                };

                return newAssistantObj;

            }
            else
            {
                Console.WriteLine("Already have an assistant :)");

                var fileID = await UploadFileAsync(filePath, assistantID);

                // return assistantID;

                // create a thread
                var threadId = await CreateThread();
                // var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(threadId);
                // Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
                 var newAssistantObj = new AssistantObj
                {
                    AssistantID = assistantID,
                    ThreadID = threadId,
                    Username = "singletonUser"
                    // Initialize other fields as necessary
                };

                return newAssistantObj;
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"An error occurred while creating the assistant: {ex.Message}");
            return null;
        }

    }

    public async Task<string> UploadFileAsync(string filePath, string assistantID)
    {
        var filesList = await _assistantApi.AssistantsEndpoint.ListFilesAsync(assistantID);
        if (filesList.Items.Count == 0)
        {
            Console.WriteLine($"no files ->{filesList.Items.Count}");
            // Upload and attach a file to the assistant.
            await File.WriteAllTextAsync(filePath, "Gender and Social Networks");
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);
            var assistantFile = await assistant.UploadFileAsync(filePath);
            //Would be good if we could remove either one of these two same api calls
            var updatedFilesList = await _assistantApi.AssistantsEndpoint.ListFilesAsync(assistantID);
            foreach (var file in updatedFilesList.Items)
            {
                Console.WriteLine($"{file.AssistantId}'s file -> {file.Id}");
                Console.WriteLine($"{file.AssistantId}'s file -> {file.Object}");
                Console.WriteLine($"{file.AssistantId}'s file -> {file.AssistantId}");
                Console.WriteLine($"{file.AssistantId}'s file -> {file.CreatedAt}");
                Console.WriteLine($"{file.AssistantId}'s file -> {file.Client}");
                Console.WriteLine($"{file.AssistantId}'s file -> {file.Client}");
            }
            return assistantFile;
        }
        return filesList.Items[0];
    }

    public async Task<List<MessageResponse>> ProcessUserQueryAndFetchResponses(string userQuery)
    {
        var messages = new List<MessageResponse>();

        try
        {
            var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(_threadId);
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(_assistantId);

            var request = new CreateMessageRequest($"{userQuery}");
            var newMessage = await _assistantApi.ThreadsEndpoint.CreateMessageAsync(thread.Id, request);

            if (newMessage != null)
            {
                var run = await thread.CreateRunAsync(assistant);

                while ($"{run.Status}" != "Completed")
                {
                    await Task.Delay(1000); // Wait for a bit before polling again
                    run = await run.UpdateAsync();
                }

                var messageList = await ListMessages(thread);
                messages.AddRange(messageList.Items);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Consider how you want to handle errors. You might want to return an error message within your messages list or handle it differently.
        }

        return messages;
    }

    public async Task<List<MessageResponse>> FetchMessageList(string threadID) {
        var messages = new List<MessageResponse>();
        try
        {
            var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(_threadId);
            var messageList = await ListMessages(thread);
            messages.AddRange(messageList.Items);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            // Consider how you want to handle errors. You might want to return an error message within your messages list or handle it differently.
        }
        return messages;
    }

    public async Task<string> checkAssistant()
    {

        var assistantsList = await _assistantApi.AssistantsEndpoint.ListAssistantsAsync();
        if (assistantsList.Items.Count == 0)
        {
            Console.WriteLine("No assistants found.");
            Console.WriteLine(assistantsList.Items.Count);
            return "No assistants found";
        }

        Console.WriteLine($"Found {assistantsList.Items.Count} assistants:");
        foreach (var assistant in assistantsList.Items)
        {
            // Log detailed information about each assistant
            Console.WriteLine($"ID: {assistant.Id}");
            Console.WriteLine($"Name: {assistant.Name}");
            Console.WriteLine($"Model: {assistant.Model}");
            Console.WriteLine($"CreatedAt: {assistant.CreatedAt}");
            Console.WriteLine($"Number of files: {assistant.FileIds.Count}");
            foreach (var fileId in assistant.FileIds)
            {
                Console.WriteLine($"fileId: {fileId}");
            }
            foreach (var tool in assistant.Tools)
            {
                Console.WriteLine($"Tools:{tool.Type}");
            }

            Console.WriteLine("-------------");
        }

        // Assuming you want to return the ID of the first assistant as before
        return assistantsList.Items[0].Id; 
    }

    //If we want to delete all assistants - not used right now
    private async Task<String> deleteAssistant()
    {
        Console.WriteLine("inside delete method");
        var assistantsList = await _assistantApi.AssistantsEndpoint.ListAssistantsAsync();

        foreach (var assistant in assistantsList.Items)
        {
            Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
            var isDeleted = await _assistantApi.AssistantsEndpoint.DeleteAssistantAsync(assistant);
        }
        return "deleted";
    }

    private async Task<string> CreateThread()
    {
        Console.WriteLine("Creating a thread");
        var thread = await _assistantApi.ThreadsEndpoint.CreateThreadAsync();
        Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");

        return thread.Id;

    }
    private async Task<ListResponse<MessageResponse>> ListMessages(ThreadResponse? thread)
    {
        if (thread == null)
        {
            throw new ArgumentNullException(nameof(thread), "Thread cannot be null.");
        }

        var messageList = await thread.ListMessagesAsync();
        return messageList;
    }

}
