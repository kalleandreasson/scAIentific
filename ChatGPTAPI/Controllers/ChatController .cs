using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ChatGPTAPI.Models;
using ChatGPTAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Ensure this using directive is correct for your project structure

namespace ChatGPTAPI.Controllers;

[ApiController, Authorize]
[Route("chat")]
public class ChatController  : ControllerBase
{
    private readonly OpenAIService _openAIApiService;
    private readonly ChatService _chatService;
    private readonly InAppFileSaverService _inAppFileSaver;
    private readonly ILogger<ChatController > _logger;
    private readonly MongoDBService _mongoDBService;


    public ChatController (OpenAIService openAIApiService, InAppFileSaverService inAppFileSaver, ILogger<ChatController > logger, ChatService chatService, MongoDBService mongoDBService)
    {
        _openAIApiService = openAIApiService;
        _inAppFileSaver = inAppFileSaver;
        _logger = logger;
        _chatService = chatService;
        _mongoDBService = mongoDBService;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> ChatWithAssistant([FromBody] UserQuery request)
    {
        var username = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(username);
        try
        {
            var assistantObj = await _mongoDBService.GetUserIfExistsAsync(username);

            Console.Write("ChatWithAssistant() -> request.UserMessage\n");
            var messages = await _chatService.ProcessUserQueryAndFetchResponses(request.UserMessage, assistantObj.ThreadID, assistantObj.AssistantID);

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
        var username = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(username);
        try
        {
            var user = await _mongoDBService.GetUserIfExistsAsync(username);
            if (user == null)
            {
                return NotFound("Singleton user not found.");
            }

            var messages = await _chatService.FetchMessageList(user.ThreadID);
            return Ok(new { Messages = messages });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during chat with assistant.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

        private async Task<string> TokenCheck(string userName)
    {
        UserObj tokenCheck = await _mongoDBService.validateUser(userName);
        if (string.IsNullOrEmpty(userName) || tokenCheck.Username == null)
        {
            throw new ArgumentException("Invalid token");
        }
        return tokenCheck.Username;
    }

}