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
    private readonly MongoDBService _mongoDBService;


    public ChatGPTAPIController(OpenAIService openAIApiService, InAppFileSaverService inAppFileSaver, ILogger<ChatGPTAPIController> logger, AssistantService assistantService, MongoDBService mongoDBService)
    {
        _openAIApiService = openAIApiService;
        _inAppFileSaver = inAppFileSaver;
        _logger = logger;
        _assistantService = assistantService;
        _mongoDBService = mongoDBService;
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

            AssistantObj assistantObj = await _assistantService.CreateAssistant(savedFilePath);
            await _mongoDBService.SaveAssistantAsync(assistantObj);
            Console.WriteLine("Return in the controller");
            Console.WriteLine(assistantObj);

            return Ok("success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving or processing the file.");
            return StatusCode(500, "There was an error processing the file.");
        }

    }

    [HttpPost("assistant-chat")]
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

    [HttpGet("assistant-chat")]
    public async Task<IActionResult> ChatWithAssistant()
    {
        try
        {
            var user = await _mongoDBService.GetSingletonUser();
            if (user == null)
            {
                return NotFound("Singleton user not found.");
            }

            var messages = await _assistantService.FetchMessageList(user.ThreadID);
            return Ok(messages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during chat with assistant.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpGet("assistant-check")]
    public async Task<IActionResult> AssistantCheck()
    {
        try
    {   
        var username = "singletonUser";
        var user = await _mongoDBService.GetUserIfExistsAsync(username);

        if (user == null)
        {
            // No user found with the provided username
            return NotFound($"No user found with username: {username}");
        }

        // User found, return the user object
        return Ok(user.AssistantID);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "An unexpected error occurred while trying to fetch the user.");
        return StatusCode(500, "An unexpected error occurred.");
    }
    }


    [HttpGet("dbtest")]
    public async Task<IActionResult> Dbtest()
    {
        try
        {
            var newUser = new AssistantObj
            {
                AssistantID = "john_doe",
                ThreadID = "john@example.com"
                // Initialize other fields as necessary
            };

            await _mongoDBService.SaveAssistantAsync(newUser);
            return Ok("This user was added to the database: " + newUser);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred during chat with assistant.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

}