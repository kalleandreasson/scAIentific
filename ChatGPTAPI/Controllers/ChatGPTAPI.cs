using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ChatGPTAPI.Models;


namespace ChatGPTAPI.Controllers;

[ApiController]
[Route("research-front")]
public class ChatGPTAPI : ControllerBase
{
    private readonly OpenAIService _openAIApiService;
    // Constructor injection of OpenAIApiService
    public ChatGPTAPI(OpenAIService openAIApiService)
    {
        _openAIApiService = openAIApiService;
    }


    [HttpGet]
    public IActionResult Get()
    {
        var response = new { Message = "Hello! You are talking to the API that will generate research front using AI" };
        return Ok(response);
    }


    // Endpoint to generate research front using chat GPT
    [HttpPost("generate")]
    public async Task<IActionResult> FindResearchFront([FromBody] FindResearchFrontRequest request)
    {
        // Ensure that the request parameters are not null
    string systemMessage = request.SystemMessage ?? "string";
    string userMessage = request.UserMessage ?? "Default User Message";
      
        // Call the OpenAI service with the system message and the user message
    string response = await _openAIApiService.CreateChatCompletionAsync(systemMessage, userMessage);

        // Parse the JSON to extract message.content
    var parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(response);
    string messageContent = parsedResponse?.choices?[0]?.message?.content ?? "No response";
        // Parse the response and return it
         // Return only the message content
    return Ok(messageContent);
    }
}
