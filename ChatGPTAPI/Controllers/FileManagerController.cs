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
            Console.WriteLine($"the uploadFile method in the controller, user id {userName}");
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided or file is empty.");
            }
            var savedFilePath = await _inAppFileSaver.Save(file, "files");

            string fileId = await _fileManagerService.ReplaceAssistantFile(userName, savedFilePath, fileName);

            // Placeholder for actual upload logic
            _logger.LogInformation($"Uploading file for user {userName}. File ID: {fileId}");
            return Ok(new { Message = $"File for user {userName} uploaded successfully.", FileId = fileId });
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
