using System.Net.Http.Headers;
using System.Text;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Audio;
using OpenAI.Threads;

public class FileManagerService
{
    private readonly MongoDBService _mongoDBService;
    private readonly HttpClient _httpClient;
    private readonly OpenAIClient _assistantApi;
    private readonly string _apiKey;

    public FileManagerService(HttpClient httpClient, IOptions<OpenAIServiceOptions> options, MongoDBService mongoDBService)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        var settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        _apiKey = settings.ApiKey ?? throw new ArgumentNullException(nameof(settings.ApiKey));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _assistantApi = new OpenAIClient(_apiKey);
        _mongoDBService = mongoDBService;
    }

    public async Task<string> UploadFile(string userName)
    {
        // ToDO: Add the userId to the userObject in the database and use it instead of username
        // ToDo: modify the file saver to save the file by the userId
        // ToDo: code a method that will find user file by the userID in the wwwroot and return the filePath
        var user = _mongoDBService.GetUserIfExistsAsync(userName);
        string assistantID = user.Result.AssistantID;

        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);


        // Delete the file from the api 
        bool isDeleted = await DeleteFile(userName);

        if (isDeleted == true)
        {
            string filePath = "wwwroot/files/Gender-Gergei.docx";

            // upload the file to the assistant
            await File.WriteAllTextAsync(filePath, "Gender and Social Networks");
            var assistantFile = await assistant.UploadFileAsync(filePath);
            string uploadedFileId = assistantFile.Id;
            if (assistantFile.Id != null)
            {
                await _mongoDBService.ReplaceFileIdForUserAsync(userName, uploadedFileId);
                // ToDo: delete the uploaded file from wwwroot folder 
            }
            return uploadedFileId;
        }
        else
        {
            return "the file was not deleted";
        }
    }


    // Gets the userFileId from the database and deletes it from the assistant in the api.
    public async Task<bool> DeleteFile(string userName)
    {
        var user = _mongoDBService.GetUserIfExistsAsync(userName);
        string assistantID = user.Result.AssistantID;
        string userFileID = user.Result.FileID;
        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);

        // isDeleted returns true if the file was deleted from the api successfully
        bool isDeleted = await assistant.DeleteFileAsync(userFileID);

        if (isDeleted == true)
        {
            Console.WriteLine(isDeleted);
            return isDeleted;
        }
        else
        {
            Console.WriteLine($"deleting status = {isDeleted}");
            return isDeleted;
        }
    }

    
    public async Task<string> UploadFileToAssistant(string userName)
    {
        Console.WriteLine($"uploading service");
        var user = _mongoDBService.GetUserIfExistsAsync(userName);
        string assistantID = user.Result.AssistantID;
        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);


        string filePath = "wwwroot/files/Gender-Gergei.docx";
        await File.WriteAllTextAsync(filePath, "Gender and Social Networks");
        var assistantFile = await assistant.UploadFileAsync(filePath);
        Console.WriteLine($"{assistantFile.AssistantId}'s file -> {assistantFile.Id}");




        return assistantFile.Id;
    }
    public async Task<int> ListAssistantFiles(string userName)
    {
        Console.WriteLine("list service");
        var user = _mongoDBService.GetUserIfExistsAsync(userName);
        string assistantID = user.Result.AssistantID;

        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);

        var filesList = await assistant.ListFilesAsync();
        foreach (var file in filesList.Items)
        {

            Console.WriteLine($"{file.AssistantId}'s file -> {file.Id}");
        }

        return filesList.Items.Count;
    }
    public async Task<string> RetrieveFileFromAssistant(string userName)
    {
        var user = _mongoDBService.GetUserIfExistsAsync(userName);
        string assistantID = user.Result.AssistantID;
        string databaseFileID = user.Result.FileID;

        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);

        var assistantFile = await assistant.RetrieveFileAsync(databaseFileID);

        Console.WriteLine($"{assistantFile.AssistantId}'s file -> {assistantFile.Id}");

        return "fileID";
    }


}