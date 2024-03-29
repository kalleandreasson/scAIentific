using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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

        public FileManagerController(ILogger<FileManagerController> logger, FileManagerService fileManagerService)
        {
            _logger = logger;
            _fileManagerService = fileManagerService;
        }


        [HttpPost("upload/{userName}")]
        public async Task<IActionResult> UploadFile(string userName, IFormFile file)
        {
             Console.WriteLine($"the delete method, user id {userName}");
            // Implementation for uploading a file for the given userName.
            // Get the assistantId from the database.
            // check if there is a file on the assistant in the api and in the database
            // if a file is found:
            // use the assistantID to delete it from both database/api and upload the new one 
            //save the new fileId in the database
            // if the file is not found use the assistantID to upload the file
            // Return the file id and success  
            string fileId = "the one that is returned from the api";

            // Placeholder for actual upload logic
            _logger.LogInformation($"Uploading file for user {userName}. File ID: {fileId}");
            return Ok(new { Message = $"File for user {userName} uploaded successfully.", FileId = fileId });
        }

        [HttpDelete("delete/{userName}")]
        public async Task<IActionResult> DeleteFile(string userName)
        {

            Console.WriteLine($"the delete method, user id {userName}");

           bool isDeleted = await _fileManagerService.DeleteFile(userName);
            // Log the file deletion
            _logger.LogInformation($"Deleting file for user {userName}. File ID: ");

            // After successful deletion, return the fileId along with a confirmation message.
            return Ok(new { Message = $"File for user {userName} deleted successfully." });
        }


         [HttpPost("uploadtoassistant/{userName}")]
        public async Task<IActionResult> UploadFileToAssistant(string userName)
        {
      
            string uploadedFileId = await _fileManagerService.UploadFileToAssistant(userName);
           

            // Placeholder for actual upload logic
            _logger.LogInformation($"Uploading file with the id {uploadedFileId} for user {userName}.");
            return Ok(new { Message = $"File for user {userName} with the file id {uploadedFileId} was uploaded successfully." });
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
