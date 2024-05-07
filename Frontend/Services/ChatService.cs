using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Frontend.Services;
using Frontend.Models;
using System.Net.Http.Headers;


namespace Frontend.Services
{
    public class ChatService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly string _chatEndpoint;
        private readonly string _chatHistoryEndpoint;
        private SessionService _sessionService;

        public ChatService(HttpClient httpClient, IConfiguration configuration, SessionService sessionService)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            string apiBaseUrl = configuration.GetValue<string>("APIBaseUrl");

            // Construct the ChatEndpoint using the base URL from the configuration
            _chatHistoryEndpoint = $"{apiBaseUrl}chat/chat-history";
            _chatEndpoint = $"{apiBaseUrl}chat/send-message";
            _sessionService = sessionService;
        }

        public async Task<ChatResponse> GetChatHistoryAsync()
        {
            try
            {
                var token = _sessionService.GetToken(); 

                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("No authentication token available.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(_chatHistoryEndpoint);

                response.EnsureSuccessStatusCode();

                // Deserialize the JSON response to a List<ChatMessage>
                ChatResponse chatHistory = await response.Content.ReadFromJsonAsync<ChatResponse>();

                if (chatHistory != null)
                {
                    foreach (var message in chatHistory.Messages)
                    {
                        var dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(message.CreatedAt);
                        message.CreatedAtDateTime = dateTimeOffset.LocalDateTime;
                    }
                }
                return chatHistory;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"HTTP request exception: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
                throw;
            }
        }


        public async Task<ChatResponse> PostUserQueryAsync(ChatRequest chatRequest)
        {
            if (chatRequest == null)
                throw new ArgumentNullException(nameof(chatRequest));

            try
            {

                var token = _sessionService.GetToken();

                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("No authentication token available.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

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
