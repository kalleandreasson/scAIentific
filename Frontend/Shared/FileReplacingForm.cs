using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;

namespace Frontend.Shared
{
    public partial class FileReplacingForm
    {
        [Inject]
        public IConfiguration? Configuration { get; set; }
        [Inject]
        public FileUploadingService? FileUploadingService { get; set; }
        [Inject]
        public ILogger<FileReplacingForm>? Logger { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        private ReplaceFile replaceFile = new ReplaceFile();
        private string flashMessage = "";

        private string? errorMessage;
        private List<string> uploadedFiles = new();

        private async Task HandleValidSubmit()
        {
            flashMessage = "";
            errorMessage = "";
            if (!filesToUpload.Any())
            {
                errorMessage = "Please upload a file.";
                return;
            }
            try
            {
                string apiBaseUrl = config["APIBaseUrl"];
                string userName= "singletonUser";
                string apiUrl = $"{apiBaseUrl}FileManager/upload/{userName}?fileName={Uri.EscapeDataString(replaceFile.FileName)}";
                // Explicitly declare the tuple types instead of using var
                (bool isSuccess, string latestErrorMessage) = await FileUploadingService.SendDataAndFileToApi(filesToUpload, apiUrl);

                if (isSuccess)
                {
                    flashMessage = $"Good! Now you have replaced the old file with \"{replaceFile.FileName}\" file successfully!";
                    uploadedFiles.Clear();
                    NavigationManager.NavigateTo("/");
                    // Trigger any success actions like navigating to another page or showing a success message
                }
                else
                {
                    errorMessage = latestErrorMessage;
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Failed to submit replacing file form.");
                errorMessage = "Failed to create the assistant due to an unexpected error. Please try again later.";
            }
            finally
            {
                // Reset replaceFile model for new input
                replaceFile = new ReplaceFile();
            }
        }

    }
}