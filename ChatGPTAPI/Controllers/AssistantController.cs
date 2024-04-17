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

        try
        {
            string userAssistant = await _assistantService.GetUserAssistantAsync(userName);
            return new JsonResult(new { assistant_id = userAssistant })
            {
                StatusCode = StatusCodes.Status200OK
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred while trying to fetch the user.");
            return StatusCode(500, "An unexpected error occurred.");
        }
    }

    [HttpPost("create-assistant")]
    public async Task<IActionResult> CreateAssistant([FromForm] IFormFile file, [FromForm] string researchArea)
    {
        var userName = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(userName);
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file provided or file is empty.");
        }

        var savedFilePath = await _inAppFileSaver.Save(userName, file, "files");
        var result = await _assistantService.CreateAssistantWithFileUploadAndThread(savedFilePath, researchArea, userName);

        if (!result.Success)
        {
            if (result.ErrorMessage == "User already has an assistant")
            {
                return StatusCode(409, new { title = "Conflict", status = 409, detail = "User already has an assistant" });
            }

            return StatusCode(500, new { title = "Internal Server Error", status = 500, detail = result.ErrorMessage });
        }

        return new JsonResult(new { assistant_id = result.Data.AssistantID })
        {
            StatusCode = StatusCodes.Status201Created
        };
    }


    [HttpDelete("delete-assistant")]
    public async Task<IActionResult> DeleteAssistant()
    {
        var userName = await TokenCheck(User.FindFirst(ClaimTypes.Name)?.Value);
        Console.WriteLine(userName);
        try
        {
            var deletionStatus = await _assistantService.DeleteUserAssistantAndThreadsFromApiAndDB(userName);
            
            if (deletionStatus.StartsWith("Successfully deleted"))
            {
                return NoContent(); 
            }
            else
            {
                return new JsonResult(new { Message = deletionStatus })
                {
                    StatusCode = StatusCodes.Status404NotFound 
                };
            }
        }
        catch (KeyNotFoundException)
        {
            _logger.LogError($"No assistant found for username: {userName}");
            return NotFound(new { Message = "No assistant found for username: " + userName });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, $"Invalid operation for username: {userName}. Error: {ex.Message}");
            return BadRequest(new { Message = $"Invalid operation for username: {userName}." });
        }
        catch (Exception ex)
        {
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