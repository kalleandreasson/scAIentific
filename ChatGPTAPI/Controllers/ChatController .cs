using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ChatGPTAPI.Models;
using ChatGPTAPI.Services; // Ensure this using directive is correct for your project structure

namespace ChatGPTAPI.Controllers;

[ApiController]
[Route("chat")]
public class ChatController  : ControllerBase
{
    private readonly OpenAIService _openAIApiService;
    private readonly AssistantService _assistantService;
    private readonly InAppFileSaverService _inAppFileSaver;
    private readonly ILogger<ChatController > _logger;
    private readonly MongoDBService _mongoDBService;


    public ChatController (OpenAIService openAIApiService, InAppFileSaverService inAppFileSaver, ILogger<ChatController > logger, AssistantService assistantService, MongoDBService mongoDBService)
    {
        _openAIApiService = openAIApiService;
        _inAppFileSaver = inAppFileSaver;
        _logger = logger;
        _assistantService = assistantService;
        _mongoDBService = mongoDBService;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> ChatWithAssistant([FromBody] UserQuery request)
    {
        try
        {
            var username = "singletonUser";
            var assistantObj = await _mongoDBService.GetUserIfExistsAsync(username);

            Console.Write("ChatWithAssistant() -> request.UserMessage\n");
            var messages = await _assistantService.ProcessUserQueryAndFetchResponses(request.UserMessage, assistantObj.ThreadID, assistantObj.AssistantID);

            return Ok(new { Messages = messages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during chat with assistant.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("chat-history")]
    public async Task<IActionResult> ChatWithAssistant()
    {
        try
        {
            var username = "singletonUser";
            var user = await _mongoDBService.GetUserIfExistsAsync(username);
            if (user == null)
            {
                return NotFound("Singleton user not found.");
            }

            var messages = await _assistantService.FetchMessageList(user.ThreadID);
            return Ok(new { Messages = messages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during chat with assistant.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

}