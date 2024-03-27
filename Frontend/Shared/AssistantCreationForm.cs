using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Frontend.Models;
using Frontend.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace Frontend.Shared
{
    public partial class AssistantCreationForm
    {
        [Inject]
        public AssistantCreationService? AssistantCreationService {get; set;}
        private UserResearch newResearch = new UserResearch();
        private string flashMessage = "";

        private int maxAllowedFiles = 1;
        private long maxFileSize = 1024 * 1024 * 512; // 512 MB
        private string? errorMessage;
        private List<string> uploadedFiles = new(); // To store uploaded file names for display

        private async Task HandleValidSubmit()
        {
            if (filesToUpload.Any())
            {
                var apiBaseUrl = config["APIBaseUrl"];
                // Explicitly declare the tuple types instead of using var
                (bool isSuccess, string latestErrorMessage) = await AssistantCreationService.SendResearchAreaAndFileToApi(filesToUpload, apiBaseUrl, newResearch.ResearchArea);

                if (isSuccess)
                {
                    flashMessage = $"Good! your Assistant that is exper in \"{newResearch.ResearchArea}\"  was successfully created!";
                    uploadedFiles.Clear();
                    // Trigger any success actions like navigating to another page or showing a success message
                }
                else
                {
                    errorMessage = latestErrorMessage;
                }
            }
            else
            {
                errorMessage = "Please upload a file.";
            }

            // Reset research model for new input
            newResearch = new UserResearch();
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
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}