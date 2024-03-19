using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Models;


namespace Frontend.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _chatEndpoint;

        public ChatService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            string apiBaseUrl = configuration.GetValue<string>("APIBaseUrl");

            // Construct the ChatEndpoint using the base URL from the configuration
            _chatEndpoint = $"{apiBaseUrl}research-front/assistant-chat";
        }

        public async Task<ChatResponse> PostUserQueryAsync(ChatRequest chatRequest)
        {
            if (chatRequest == null)
                throw new ArgumentNullException(nameof(chatRequest));

            Console.WriteLine(chatRequest.UserMessage);

            try
            {
                // Sending the request as JSON and expecting a response as JSON
                HttpResponseMessage response = await _httpClient.PostAsJsonAsync(_chatEndpoint, chatRequest);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Deserialize the JSON response to the ChatResponse object
                ChatResponse chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>();
                foreach (var message in chatResponse.Messages)
                {
                    var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(message.CreatedAt);
                    message.CreatedAtDateTime = dateTimeOffset.LocalDateTime;
                }

                return chatResponse;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the exception as required
                Console.WriteLine($"HTTP request exception: {ex.Message}");
                throw;
            }
            // Catch any other unexpected exceptions
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }
    }
}
