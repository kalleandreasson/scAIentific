using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Components.Forms;
using Frontend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class FileUploadingService
    {
        private readonly HttpClient _httpClient;
        private readonly long _maxFileSize = 1024 * 1024 * 500; // 500MB

        public FileUploadingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<(bool isSuccess, string errorMessage)> SendDataAndFileToApi(IEnumerable<IBrowserFile> filesToUpload, string apiUrl)
        {
            // Validate filesToUpload
            if (filesToUpload == null || !filesToUpload.Any())
            {
                return (false, "No files to upload.");
            }
            try
            {
                using var content = new MultipartFormDataContent();
                foreach (var file in filesToUpload)
                {
                    var fileContent = new StreamContent(file.OpenReadStream(_maxFileSize));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    content.Add(fileContent, "file", file.Name);
                }

                // Append the research area as a query parameter to the URL
                var urlWithQuery = $"{apiUrl}";

                var response = await _httpClient.PostAsync(urlWithQuery, content);
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
            catch (System.Exception ex)
            {
                return (false, $"An error occurred while sending the file to the server: {ex.Message}");
            }
        }

    }
}