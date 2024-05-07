namespace ChatGPTAPI.Services
{
    public class InAppFileSaverService
    {
        private readonly IWebHostEnvironment env;
        public InAppFileSaverService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task<string> Save(string userName,IFormFile file, string folderName)
        {

            // Get the file extension
            var fileExtension = Path.GetExtension(file.FileName);

            // Construct the new filename by appending "the userName" to the original filename, then add the file extension
            var filename = $"{userName}-{Guid.NewGuid()}{fileExtension}";

            // Combine the web root path with the folder name to get the route
            string route = Path.Combine(env.WebRootPath, folderName);

            // Create the directory if it doesn't exist
            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }

            // Combine the route with the new filename to get the full file path
            string fileRoute = Path.Combine(route, filename);

            // Save the file to the server
            using (var fileStream = new FileStream(fileRoute, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return the full path of the saved file
            return fileRoute;
        }

    }
}