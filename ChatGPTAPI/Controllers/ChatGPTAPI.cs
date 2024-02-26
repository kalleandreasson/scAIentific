using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ChatGPTAPI.Models;
using ChatGPTAPI.Services; // Ensure this using directive is correct for your project structure

namespace ChatGPTAPI.Controllers;

[ApiController]
[Route("research-front")]
public class ChatGPTAPIController : ControllerBase
{
    private readonly OpenAIService _openAIApiService;
    private readonly AssistantService _assistantService;
    private readonly InAppFileSaverService _inAppFileSaver;
    private readonly ILogger<ChatGPTAPIController> _logger;

    public ChatGPTAPIController(OpenAIService openAIApiService, InAppFileSaverService inAppFileSaver, ILogger<ChatGPTAPIController> logger, AssistantService assistantService)
    {
        _openAIApiService = openAIApiService;
        _inAppFileSaver = inAppFileSaver;
        _logger = logger;
        _assistantService = assistantService;
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
        if (request == null || string.IsNullOrEmpty(request.UserMessage))
        {
            return BadRequest("Invalid request data");
        }

        try
        {
            string systemMessage = request.SystemMessage ?? "Default System Message";
            string userMessage = request.UserMessage;

            string response = await _openAIApiService.CreateChatCompletionAsync(systemMessage, userMessage);
            var parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(response);
            string messageContent = parsedResponse?.choices?[0]?.message?.content ?? "No response";

            if (string.IsNullOrEmpty(messageContent))
            {
                return NotFound("The response from AI was empty");
            }

            return Ok(messageContent);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Error parsing response from AI service.");
            return StatusCode(500, "There was an error processing the AI response.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("generateByFile")]
    public async Task<IActionResult> FindResearchFrontByFile([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided or file is empty.");
        }
        try
        {
            var savedFilePath = await _inAppFileSaver.Save(file, "files");
   
            string response = await _assistantService.CreateAssistant(savedFilePath);

            return Ok("success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving or processing the file.");
            return StatusCode(500, "There was an error processing the file.");
        }

    }
}