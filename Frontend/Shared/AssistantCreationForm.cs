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
        public IConfiguration? Configuration { get; set; }
        [Inject]
        public FileUploadingService? FileUploadingService { get; set; }
        [Inject]
        public ILogger<AssistantCreationForm>? Logger { get; set; }
        [Inject]
        public NavigationManager? NavigationManager { get; set; }

        private UserResearch newResearch = new UserResearch();
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
                string apiUrl = $"{apiBaseUrl}research-front/generateByFile?researchArea={Uri.EscapeDataString(newResearch.ResearchArea)}";
                // Explicitly declare the tuple types instead of using var
                (bool isSuccess, string latestErrorMessage) = await FileUploadingService.SendDataAndFileToApi(filesToUpload, apiUrl);

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

    }
}