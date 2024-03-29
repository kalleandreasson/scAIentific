using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using ChatGPTAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ChatGPTAPI.Controllers
{
    [Route("[controller]")]
    public class FileManagerController : Controller
    {
        private readonly ILogger<FileManagerController> _logger;
        private readonly FileManagerService _fileManagerService;
        private readonly InAppFileSaverService _inAppFileSaver;

        public FileManagerController(ILogger<FileManagerController> logger, FileManagerService fileManagerService, InAppFileSaverService inAppFileSaver)
        {
            _logger = logger;
            _inAppFileSaver = inAppFileSaver;
            _fileManagerService = fileManagerService;
        }


        // filename is to give the file a name when updated in the api
        [HttpPost("upload/{userName}")]
        public async Task<IActionResult> UploadFile(string userName, string fileName, IFormFile file)
        {
            _logger.LogInformation($"Starting file upload process for user '{userName}'.");

            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("UploadFile called with no file provided or file is empty.");
                return BadRequest("No file provided or file is empty.");
            }

            string savedFilePath;
            try
            {
                savedFilePath = await _inAppFileSaver.Save(userName, file, "files");
                _logger.LogInformation($"File successfully saved to {savedFilePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save the file.");
                return BadRequest("Failed to save the file.");
            }

            if (string.IsNullOrWhiteSpace(savedFilePath))
            {
                // If savedFilePath is null or empty, assume the save operation failed.
                _logger.LogWarning("File save operation did not return a valid path.");
                return BadRequest("Failed to save the file.");
            }

            try
            {
                string fileId = await _fileManagerService.ReplaceAssistantFile(userName, savedFilePath, fileName);
                _logger.LogInformation($"File for user '{userName}' replaced successfully. File ID: {fileId}");
                return Ok(new { Message = "File uploaded and replaced successfully.", FileId = fileId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to replace the assistant file for user '{userName}'.");
                // Consider the appropriate response based on your error handling policy
                return StatusCode(500, "Failed to replace the assistant file.");
            }
        }

        [HttpPost("listassistantfiles/{userName}")]
        public async Task<IActionResult> listAssistantFiles(string userName)
        {

            // await _fileManagerService.UploadFileToAssistant(userName);
            Console.WriteLine("list controller");
            int numberOIfFiles = await _fileManagerService.ListAssistantFiles(userName);

            // Placeholder for actual upload logic
            _logger.LogInformation($"Listing");
            return Ok(new { Message = $"File for user {userName} are {numberOIfFiles}" });
        }
    }
}
