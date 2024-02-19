using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Audio;

public class AssistantService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;

    public AssistantService(HttpClient httpClient, IOptions<OpenAIServiceOptions> options)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
    }

    public async Task<string> CreateAssistant(IFormFile file)
    {
        Console.WriteLine("inside Assistant method");
        using var api = new OpenAIClient(_apiKey);
        var assistantID = await checkAssistant();
        if (assistantID == "false")
        {
            var request = new CreateAssistantRequest("gpt-3.5-turbo-1106");
            var assistantCreate = await api.AssistantsEndpoint.CreateAssistantAsync(request);
            Console.WriteLine("Created Assistant :) ");
            return assistantCreate;
        }
        else
        {
            Console.WriteLine("Already have assistant :) ");
            return assistantID;
        }
    }

    private async Task<string> checkAssistant()
    {
        using var api = new OpenAIClient(_apiKey);
        var assistantsList = await api.AssistantsEndpoint.ListAssistantsAsync();
        if (assistantsList.Items.Count < 0)
        {
            return "false";
        }

        foreach (var assistant in assistantsList.Items)
        {
            Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
        }
        return assistantsList.Items[0];
    }

    //If we want to delete all assistants - not used right now
    private async void deleteAssistant()
    {
        using var api = new OpenAIClient(_apiKey);
        var assistantsList = await api.AssistantsEndpoint.ListAssistantsAsync();

        foreach (var assistant in assistantsList.Items)
        {
            Console.WriteLine($"{assistant} -> {assistant.CreatedAt}");
            var isDeleted = await api.AssistantsEndpoint.DeleteAssistantAsync(assistant);
        }
    }

}
