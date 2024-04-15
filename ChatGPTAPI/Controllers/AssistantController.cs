using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ChatGPTAPI.Models;
using ChatGPTAPI.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims; // Ensure this using directive is correct for your project structure

namespace ChatGPTAPI.Controllers;


[ApiController, Authorize]
[Route("research-front")]
public class AssistantController : ControllerBase
{
    private readonly AssistantService _assistantService;
    private readonly InAppFileSaverService _inAppFileSaver;
    private readonly ILogger<AssistantController> _logger;
    private readonly MongoDBService _mongoDBService;


    public AssistantController(InAppFileSaverService inAppFileSaver, ILogger<AssistantController> logger, AssistantService assistantService, MongoDBService mongoDBService)
    {
        _inAppFileSaver = inAppFileSaver;
        _logger = logger;
        _assistantService = assistantService;
        _mongoDBService = mongoDBService;
    }

    [HttpGet("get-assistant")]
    public async Task<IActionResult> getUserAssistant()
    {
        var userName = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(userName);
        try
        {

            Console.WriteLine("assistant check");
            string userAssistant = await _assistantService.GetUserAssistantAsync(userName);

            // User found, return the user object
            return Ok(userAssistant);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while trying to fetch the user.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    //Rename method files
    [HttpPost("create-assistant")]
    public async Task<IActionResult> CreateAssistant([FromForm] IFormFile file, [FromQuery] string researchArea)
    {
        var userName = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(userName);
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided or file is empty.");
        }
        try
        {
            var savedFilePath = await _inAppFileSaver.Save(userName, file, "files");
            var assistantObj = await _assistantService.CreateAssistantWithFileUploadAndThread(savedFilePath, researchArea, userName);

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


    [HttpDelete("delete-assistant")]
    public async Task<IActionResult> DeleteAssistant()
    {
        var userName = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(userName);
        try
        {
            var deletionStatus = await _assistantService.DeleteUserAssistantAndThreadsFromApiAndDB(userName);
            return Ok(new { Message = $"Assistant deletion for user '{userName}' was {deletionStatus}." });
        }
        catch (KeyNotFoundException knfEx)
        {
            _logger.LogError(knfEx, $"User or Assistant not found for username: {userName}");
            return NotFound(new { Message = $"User or Assistant not found for username: {userName}." });
        }
        catch (InvalidOperationException ioeEx)
        {
            _logger.LogError(ioeEx, $"Invalid operation for username: {userName}. Error: {ioeEx.Message}");
            return BadRequest(new { Message = $"Invalid operation for username: {userName}." });
        }
        catch (Exception ex)
        {
            // Log the exception and return a generic error response
            _logger.LogError(ex, $"An unexpected error occurred while deleting assistant for username: {userName}.");
            return StatusCode(500, new { Message = "An unexpected error occurred. Please try again later." });
        }
    }

    [HttpGet("get-all-assistants")]
    public async Task<IActionResult> GetAllAssistants()
    {
        try
        {
            var assistantsList = await _assistantService.ListAllApisAssistant();

            if (!assistantsList.Items.Any())
            {
                return Ok(new { Message = "No assistants found." });
            }

            return Ok(assistantsList.Items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while trying to fetch assistants.");
            return StatusCode(500, new { Message = "An unexpected error occurred." });
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