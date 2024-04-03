using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Threads;

public class ChatService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly OpenAIClient _assistantApi;
    private readonly MongoDBService _mongoDBService;
    private readonly FileManagerService _fileManagerService;
    private readonly ILogger<ChatService> _logger;


    public ChatService(HttpClient httpClient, IOptions<OpenAIServiceOptions> options, MongoDBService mongoDBService, FileManagerService fileManagerService, ILogger<ChatService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _assistantApi = new OpenAIClient(_apiKey);
        _mongoDBService = mongoDBService;
        _fileManagerService = fileManagerService;
        _logger = logger;
    }

    public async Task<List<MessageResponse>> ProcessUserQueryAndFetchResponses(string userQuery, string threadID, string assistantID)
    {
        var messages = new List<MessageResponse>();

        try
        {
            var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(threadID);
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);

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

    public async Task<List<MessageResponse>> FetchMessageList(string threadID)
    {
        var messages = new List<MessageResponse>();
        try
        {
            var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(threadID);
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


    private async Task<ListResponse<MessageResponse>> ListMessages(ThreadResponse? thread)
    {
        if (thread == null)
        {
            throw new ArgumentNullException(nameof(thread), "Thread cannot be null.");
        }

        var messageList = await thread.ListMessagesAsync();
        return messageList;
    }

    public async Task<string> RetrieveThread(string threadID)
    {
        var thread = await _assistantApi.ThreadsEndpoint.RetrieveThreadAsync(threadID);
        return thread;
    }

}
