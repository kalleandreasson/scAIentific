// BaseFileUploadComponent.razor.cs
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Frontend.Shared // Adjust the namespace if necessary
{
    public class BaseFileUploadComponent : ComponentBase
    {
        [Inject]
        public ILogger<BaseFileUploadComponent>? Logger { get; set; }

        protected List<IBrowserFile> filesToUpload = new();
        protected List<string> uploadedFiles = new();
        protected string errorMessage = string.Empty;

        protected int maxAllowedFiles = 1;
        protected long maxFileSize = 1024 * 1024 * 512; // 512 MB

        protected void LoadFiles(InputFileChangeEventArgs e)
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
