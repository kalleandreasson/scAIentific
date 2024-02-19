using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<OpenAIService> _logger;
    public OpenAIService(HttpClient httpClient, IOptions<OpenAIServiceOptions> options, ILogger<OpenAIService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<string> CreateChatCompletionAsync(string systemMessage, string userMessage)
    {
        var requestBody = new
        {
            model = "gpt-4",
            messages = new[]
            {
            new { role = "system", content = systemMessage },
            new { role = "user", content = userMessage }
        }
        };

        string jsonContent = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        try
        {
            HttpResponseMessage response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);
            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                return responseContent;
            }
            else
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("OpenAI API call failed: {ErrorContent}", errorContent);
                throw new HttpRequestException($"Error calling OpenAI API: {errorContent}");
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request to OpenAI API failed: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}
