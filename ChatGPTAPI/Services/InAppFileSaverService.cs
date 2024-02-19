namespace ChatGPTAPI.Services
{
    public class InAppFileSaverService
    {
        private readonly IWebHostEnvironment env;
        public InAppFileSaverService(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public async Task Save(IFormFile file, string folderName)
        {
            var filename = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string route = Path.Combine(env.WebRootPath, folderName);

            if (!Directory.Exists(route))
            {
                Directory.CreateDirectory(route);
            }

            string fileRoute = Path.Combine(route, filename);

            using (var fileStream = new FileStream(fileRoute, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
    }
}