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

    public ChatGPTAPIController(OpenAIService openAIApiService, InAppFileSaverService inAppFileSaver, AssistantService assistantService)
    {
        _openAIApiService = openAIApiService;
        _inAppFileSaver = inAppFileSaver;
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
        string systemMessage = request.SystemMessage ?? "string";
        string userMessage = request.UserMessage ?? "Default User Message";
      
        string response = await _openAIApiService.CreateChatCompletionAsync(systemMessage, userMessage);

        var parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(response);
        string messageContent = parsedResponse?.choices?[0]?.message?.content ?? "No response";

        return Ok(messageContent);
    }

    [HttpPost("generateByFile")]
    public async Task<IActionResult> FindResearchFrontByFile([FromForm] IFormFile file)
    {
        await _inAppFileSaver.Save(file, "files");
        Console.WriteLine(file);
        string response = await _assistantService.CreateAssistant(file);
        Console.WriteLine(response);
        return Ok("success");
    }
}