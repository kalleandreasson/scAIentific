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

public async Task<string> CreateAssistant(string filePath)
{
    Console.WriteLine(filePath);
    var tools = new List<Tool> { Tool.Retrieval };

    Console.WriteLine("inside Assistant method");
    try
    {
        using var api = new OpenAIClient(_apiKey);
        var assistantID = await checkAssistant();
        if (assistantID == "false")
        {
            var request = new CreateAssistantRequest("gpt-4-turbo-preview", "Research expert", null, "You are great at finding research at the forefront, use the information that is provided in the file to form a good response.", tools);
            var assistantCreate = await api.AssistantsEndpoint.CreateAssistantAsync(request);
            Console.WriteLine("Created Assistant :)");
            return assistantCreate; // Assuming this returns a string that represents something meaningful (e.g., Assistant ID)
        }
        else
        {
            Console.WriteLine("Already have an assistant :)");
            return assistantID;
        }
    }
    catch (Exception ex)
    {
        // Log the exception or handle it as needed
        Console.WriteLine($"An error occurred while creating the assistant: {ex.Message}");
        return "Error"; // Return a meaningful error message or handle it differently depending on your application's needs
    }
}


private async Task<string> checkAssistant()
{
    using var api = new OpenAIClient(_apiKey);
    var assistantsList = await api.AssistantsEndpoint.ListAssistantsAsync();
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

        Console.WriteLine("-------------"); 
    }

    // Assuming you want to return the ID of the first assistant as before
    return assistantsList.Items[0].Id;
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
