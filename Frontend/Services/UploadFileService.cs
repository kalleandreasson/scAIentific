
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using Frontend.Models;

namespace Frontend.Services
{
    public class UploadFileService (HttpClient httpClient, ExcelService excelService)
    {
    private readonly HttpClient httpClient = httpClient;
    private readonly ExcelService excelService = excelService;
    private long maxFileSize = 1024 * 1024 * 500; // 500MB

    public async Task<List<ResearchModel>> ProcessExcelFile(IBrowserFile file)
    {
        try
        {
            using var stream = file.OpenReadStream(maxFileSize);
            var models = await excelService.ReadExcelAsync(stream);
            return models; // Return the list of models to the calling method
        }
        catch
        {
            // Log or handle errors as needed
            return new List<ResearchModel>(); // Return an empty list on error
        }
    }

    public async Task<(bool isSuccess, string errorMessage)> SendFileToApi(IEnumerable<IBrowserFile> filesToUpload, string apiBaseUrl)
    {
        try
        {
            using var content = new MultipartFormDataContent();
            foreach (var file in filesToUpload)
            {
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                content.Add(fileContent, "file", file.Name);
            }
            var response = await httpClient.PostAsync($"{apiBaseUrl}research-front/generateByFile", content);
            if (response.IsSuccessStatusCode)
            {
                return (true, string.Empty);
            }
            else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                return (false, $"Failed to upload file: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            return (false, $"An error occurred while sending the file to the server: {ex.Message}");
        }
    }
}
    
}