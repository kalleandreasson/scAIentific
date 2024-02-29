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
                if (filesList.Items.Count == 0)
                {
                    Console.WriteLine($"no files ->{filesList.Items.Count}");
                    // Upload and attach a file to the assistant.
                    await File.WriteAllTextAsync(filePath, "Gender and Social Networks");
                    var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);
                    var assistantFile = await assistant.UploadFileAsync(filePath);
                    return assistantFile;
                }
                else
                {
                    Console.WriteLine("The assistant has a file");
                    foreach (var file in filesList.Items)
                    {
                        Console.WriteLine($"{file.AssistantId}'s file -> {file.Id}");
                        Console.WriteLine($"{file.AssistantId}'s file -> {file.Object}");
                        Console.WriteLine($"{file.AssistantId}'s file -> {file.AssistantId}");
                        Console.WriteLine($"{file.AssistantId}'s file -> {file.CreatedAt}");
                        Console.WriteLine($"{file.AssistantId}'s file -> {file.Client}");
                    }
                    // var threadId = CreateThread();
                    var threadId = "thread_RPoKBh47laYWbz93FjPh2MMW";
                    // var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(threadId);
                    // Console.WriteLine($"Retrieve thread {thread.Id} -> {thread.CreatedAt}");
                    // var request = new CreateMessageRequest("How many abstracts are there in the file");
                    // var message = await _assistantApi.ThreadsEndpoint.CreateMessageAsync(thread.Id, request);

                    // Console.WriteLine($"{message.Id}: {message.Role}: {message.PrintContent()}");
                    return threadId;
                }
            }
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
        return thread.Id;

        // Retrieve thread thread_RPoKBh47laYWbz93FjPh2MMW -> 2024-02-29 13:22:03
    }
}
