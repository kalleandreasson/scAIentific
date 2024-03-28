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

        public FileManagerController(ILogger<FileManagerController> logger)
        {
            _logger = logger;
        }


        [HttpPost("upload/{userId}")]
        public async Task<IActionResult> UploadFile(string userId, IFormFile file)
        {
             Console.WriteLine($"the delete method, user id {userId}");
            // Implementation for uploading a file for the given userId.
            // Get the assistantId from the database.
            // check if there is a file on the assistant in the api and in the database
            // if a file is found:
            // use the assistantID to delete it and upload the new one 
            //save the new fileId in the database
            // if the file is not found use the assistantID to upload the file
            // Return the file id and success  
            string fileId = "the one that is returned from the api";

            // Placeholder for actual upload logic
            _logger.LogInformation($"Uploading file for user {userId}. File ID: {fileId}");
            return Ok(new { Message = $"File for user {userId} uploaded successfully.", FileId = fileId });
        }

        [HttpDelete("delete/{userId}")]
        public async Task<IActionResult> DeleteFile(string userId)
        {
            Console.WriteLine($"the delete method, user id {userId}");
            // Placeholder for the logic to delete the file and retrieve its ID from the database.
            string fileId = "existing_fileId_to_be_deleted"; // Replace with actual logic to retrieve the fileId before deleting.

            // Proceed with the file deletion process, including API calls as necessary.
            string assistantID = "the retrived assistant id";

            // Log the file deletion
            _logger.LogInformation($"Deleting file for user {userId}. File ID: {fileId}");

            // After successful deletion, return the fileId along with a confirmation message.
            return Ok(new { Message = $"File for user {userId} deleted successfully.", FileId = fileId });
        }
    }
}
