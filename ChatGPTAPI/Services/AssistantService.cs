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
    private readonly string _threadId = "thread_RPoKBh47laYWbz93FjPh2MMW";
    private readonly string _assistantId = "asst_rAjmsxr5I4tTI6r5ljnjnWLs";
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

    public async Task<string> CreateAssistant(string filePath)
    {
        Console.WriteLine(filePath);
        var tools = new List<Tool> { Tool.Retrieval };

        Console.WriteLine("inside Assistant method");
        try
        {
            var assistantID = await checkAssistant();
            // asst_EAo1GvU65l60K9k7FRnjjHy6
            if (assistantID == "false")
            {
                var request = new CreateAssistantRequest("gpt-4-turbo-preview", "Research expert", null, $"You have demonstrated proficiency in analyzing abstracts of research articles to identify and find the research articles forefront in \"{_researchArea}\", Please review the information provided in the attached file. Based on your analysis, formulate a comprehensive response.", tools);
                var assistantCreate = await _assistantApi.AssistantsEndpoint.CreateAssistantAsync(request);
                Console.WriteLine($"Created Assistant: {assistantCreate}");
                return assistantCreate;
            }
            else
            {
                Console.WriteLine("Already have an assistant :)");
                // Check if the assistant has a file
                var filesList = await _assistantApi.AssistantsEndpoint.ListFilesAsync(assistantID);
                Console.WriteLine($"files -> {filesList.Items.Count}");
                // if (filesList.Items.Count == 0)
                // {
                // Console.WriteLine($"no files ->{filesList.Items.Count}");
                    // Upload and attach a file to the assistant.
                //     await File.WriteAllTextAsync(filePath, "Gender and Social Networks");
                //     var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);
                //     var assistantFile = await assistant.UploadFileAsync(filePath);
                //     return assistantFile;
                // }
                // else
                // {
                //     Console.WriteLine("The assistant has a file");
                //     foreach (var file in filesList.Items)
                //     {
                //         Console.WriteLine($"{file.AssistantId}'s file -> {file.Id}");
                //         Console.WriteLine($"{file.AssistantId}'s file -> {file.Object}");
                //         Console.WriteLine($"{file.AssistantId}'s file -> {file.AssistantId}");
                //         Console.WriteLine($"{file.AssistantId}'s file -> {file.CreatedAt}");
                //         Console.WriteLine($"{file.AssistantId}'s file -> {file.Client}");
                //         Console.WriteLine($"{file.AssistantId}'s file -> {file.Client}");
                //     }

                // create a thread
                // var threadId = CreateThread();
                //     var threadId = "thread_RPoKBh47laYWbz93FjPh2MMW";
                     // var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(threadId);
                // Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
                // Retrieve thread thread_RPoKBh47laYWbz93FjPh2MMW -> 2024-02-29 13:22:03
                //     return threadId;
                // }
                return assistantID;
            }
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"An error occurred while creating the assistant: {ex.Message}");
            return "Error"; //handle the error
        }
    }

    public async Task<string> CreateRun(string userQuery)
    {
        try
        {
            Console.WriteLine("\nCreateRun()");
            var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(_threadId);
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(_assistantId);

            // create a message and add it to the thread.
            // var request = new CreateMessageRequest($"{userQuery}");
            // var newMessage = await _assistantApi.ThreadsEndpoint.CreateMessageAsync(thread.Id, request);
            // Console.WriteLine($"{newMessage.Id}: {newMessage.Role}: {newMessage.PrintContent()}");


            // create a run 
            // var run = await thread.CreateRunAsync(assistant);
            // Console.WriteLine($"run created:runID[{run.Id}], status: {run.Status} | {run.CreatedAt}");   // [run_Xl7FtiV5lCGJgfRvCBMpsNEB] Queued | 2024-03-01 08:11:53

            // list runs
            // var runList = await thread.ListRunsAsync();
            // foreach (var run in runList.Items)
            // {
            //     Console.WriteLine($"[runID: {run.Id}] status: {run.Status} | {run.CreatedAt}");
            // }

            // CHECKING THE STATUS OF THE RUN it starts as Queued and then Completed 

            // var run = await thread.RetrieveRunAsync("run_NUTSFgxFa5XUJzdHkqrAuVyh");
            // var runUpdate = await run.UpdateAsync();
            // Console.WriteLine($"run retrieved:runID[{run.Id}], status: {run.Status} | {run.CreatedAt}");
            // run retrieved:runID[run_NUTSFgxFa5XUJzdHkqrAuVyh], status: Completed | 2024-03-01 10:22:35

            // When completed logg all the messages;
            // var messageList = await ListMessages(thread);
            // foreach (var message in messageList.Items)
            // {
            //     Console.WriteLine($"---{message.Role}---: \n{message.PrintContent()}\n ****************************\n");
            // }
            return userQuery;
            // Should return the thread messages
        }
        catch (Exception ex)
        {
            // Log the exception or handle it as needed
            Console.WriteLine($"An error occurred while creating the assistant: {ex.Message}");
            return "Error"; //handle the error
        }
    }

    private async Task<string> checkAssistant()
    {

        var assistantsList = await _assistantApi.AssistantsEndpoint.ListAssistantsAsync();
        if (assistantsList.Items.Count == 0)
        {
            Console.WriteLine("No assistants found.");
            Console.WriteLine(assistantsList.Items.Count);
            return "false";
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
            Console.WriteLine($"Tools:{assistant.Tools}");

            Console.WriteLine("-------------");
        }

        // Assuming you want to return the ID of the first assistant as before
        return assistantsList.Items[0].Id;
    }



    //If we want to delete all assistants - not used right now
    private async void deleteAssistant()
    {

        var assistantsList = await _assistantApi.AssistantsEndpoint.ListAssistantsAsync();

        foreach (var assistant in assistantsList.Items)
        {
            Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
            var isDeleted = await _assistantApi.AssistantsEndpoint.DeleteAssistantAsync(assistant);
        }
    }

    private async Task<string> CreateThread()
    {
        Console.WriteLine("Creating a thread");
        var thread = await _assistantApi.ThreadsEndpoint.CreateThreadAsync();
        Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
        // Retrieve thread thread_RPoKBh47laYWbz93FjPh2MMW -> 2024-02-29 13:22:03

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
