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
    public partial class AssistantCreationForm
    {
        [Inject]
        public AssistantCreationService? AssistantCreationService {get; set;}
        [Inject]
        public ILogger<AssistantCreationForm>? Logger { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        private UserResearch newResearch = new UserResearch();
        private string flashMessage = "";

        private int maxAllowedFiles = 1;
        private long maxFileSize = 1024 * 1024 * 512; // 512 MB
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
                var apiBaseUrl = config["APIBaseUrl"];
                // Explicitly declare the tuple types instead of using var
                (bool isSuccess, string latestErrorMessage) = await AssistantCreationService.SendResearchAreaAndFileToApi(filesToUpload, apiBaseUrl, newResearch.ResearchArea);

                if (isSuccess)
                {
                    flashMessage = $"Good! your Assistant that is expert in \"{newResearch.ResearchArea}\"  was successfully created!";
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
                Logger?.LogError(ex, "Failed to submit research assistant creation form.");
                errorMessage = "Failed to create the assistant due to an unexpected error. Please try again later.";
            }
            finally
            {
                // Reset research model for new input
                newResearch = new UserResearch();
            }
        }

        private List<IBrowserFile> filesToUpload = new(); // To hold the file in memory before upload

        private void LoadFiles(InputFileChangeEventArgs e)
        {
            errorMessage = string.Empty; // Reset the error message
            uploadedFiles.Clear(); // Clear previously uploaded files list for display
            filesToUpload.Clear(); // Clear previously selected files
            try
            {
                 if (e.FileCount > maxAllowedFiles)
                {
                    errorMessage = $"Error: Attempting to upload {e.FileCount} files, but only {maxAllowedFiles} is allowed.";
                    return;
                }

                foreach (var file in e.GetMultipleFiles(maxAllowedFiles))
                {
                    if (file.Size > maxFileSize)
                    {
                        errorMessage = $"File size exceeds the maximum limit of {maxFileSize / (1024 * 1024)} MB.";
                        return;
                    }

                    filesToUpload.Add(file);
                    uploadedFiles.Add(file.Name);
                }
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "An error occurred while processing the files.");
                errorMessage = "An unexpected error occurred while processing the files. Please try again.";
            }
        }
        
    }
}