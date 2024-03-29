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

    public async Task<string> ReplaceAssistantFile(string userName, string filePath, string fileName)
    {
        // ToDO: Add the userId to the userObject in the database and use it instead of username
        // ToDo: modify the file saver to save the file by the userId
        // ToDo: code a method that will find user file by the userID in the wwwroot and return the filePath

        // Delete the file from the api 
        string newAssistantFileId = await ReplaceAssistantFileAsync(userName, fileName, filePath);

        // then update the file reference in the database.

        if (newAssistantFileId != null)
        {
            await UpdateDatabaseFileReference(userName, newAssistantFileId);

            // ToDo: delete the uploaded file from wwwroot folder 
            return newAssistantFileId;
        }

        else
        {
            return "the file was not deleted";
        }
    }

    private async Task<string> ReplaceAssistantFileAsync(string userName, string fileName, string filePath)
    {
        try
        {
            var user = await _mongoDBService.GetUserIfExistsAsync(userName);
            if (user == null)
            {
                throw new InvalidOperationException("User does not exist.");
            }

            string assistantId = user.AssistantID;
            var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantId);

            bool isDeleted = await DeleteAssistantFile(userName);

            if (!isDeleted)
            {
                // Handle the failure of file deletion appropriately.
                return null; // Or consider throwing an exception.
            }

            await File.WriteAllTextAsync(filePath, fileName);
            var assistantFile = await assistant.UploadFileAsync(filePath);

            return assistantFile?.Id; // Safe navigation in case assistantFile is null.
        }
        catch (Exception ex)
        {
            // Consider logging the exception and handling any cleanup if necessary.
            throw; // Rethrowing the exception keeps the stack trace intact.
        }
    }


    // Gets the userFileId from the database and deletes it from the assistant in the api.
    private async Task<bool> DeleteAssistantFile(string userName)
    {
        var user = _mongoDBService.GetUserIfExistsAsync(userName);
        string assistantId = user.Result.AssistantID;
        string userFileId = user.Result.FileID;
        var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantId);


        // isDeleted returns true if the file was deleted from the api successfully
        bool isDeleted = await assistant.DeleteFileAsync(userFileId);

        if (isDeleted == true)
        {
            Console.WriteLine("Delete from the api was called");
            Console.WriteLine(isDeleted);
            return isDeleted;
        }
        else
        {
            return isDeleted;
        }
    }


    private async Task UpdateDatabaseFileReference(string userName, string newFileId)
    {
        await _mongoDBService.ReplaceFileIdForUserAsync(userName, newFileId);
    }






    // public async Task<string> UploadFileToAssistant(string userName)
    // {
    //     Console.WriteLine($"uploading service");
    //     var user = _mongoDBService.GetUserIfExistsAsync(userName);
    //     string assistantID = user.Result.AssistantID;
    //     var assistant = await _assistantApi.AssistantsEndpoint.RetrieveAssistantAsync(assistantID);


    //     string filePath = "wwwroot/files/Gender-Gergei.docx";
    //     await File.WriteAllTextAsync(filePath, "Gender and Social Networks");
    //     var assistantFile = await assistant.UploadFileAsync(filePath);
    //     Console.WriteLine($"{assistantFile.AssistantId}'s file -> {assistantFile.Id}");




    //     return assistantFile.Id;
    // }

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
}