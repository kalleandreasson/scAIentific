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

    //Create file controller with two endpoints - delete and upload file
    //Change to create assistant endpoint
    //Take file and researchArea as input
    //Rename method files
    [HttpPost("generateByFile")]
    public async Task<IActionResult> FindResearchFrontByFile([FromForm] IFormFile file, [FromQuery] string researchArea)
    {
        if (file == null || file.Length == 0)
    {
        return BadRequest("No file provided or file is empty.");
    }
    try
    {
        var savedFilePath = await _inAppFileSaver.Save("singletonUser",file, "files");
        var assistantObj = await _assistantService.CreateAssistant(savedFilePath, researchArea);

        if (assistantObj == null)
        {
            Console.WriteLine("Assistant object is null, not saving to database.");
            return StatusCode(500, "Failed to create assistant object.");
        }
        
        Console.WriteLine("Return in the controller");
        Console.WriteLine(assistantObj.AssistantID);

        return Ok(assistantObj.AssistantID);
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

    [HttpGet("assistant-check")]
    public async Task<IActionResult> AssistantCheck()
    {
        try
    {   
        var username = "singletonUser";
        var user = await _mongoDBService.GetUserIfExistsAsync(username);
        
        if (user == null)
        {
            Console.WriteLine();
            // No user found with the provided username
            return StatusCode(404, "Assistant not found");
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
    private async Task<IActionResult> Dbtest()
    {
        try
        {
            var newUser = new AssistantObj
            {
                AssistantID = "Db test",
                ThreadID = "Db test"
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

    [HttpGet("retrieveThread")]
    private async Task<IActionResult> RetreiveThread() {
        var threadID = "";
        var retrievedThread = await _assistantService.RetrieveThread(threadID);
        return Ok(retrievedThread);
    }

}