namespace CookBookBackend.Core.Services
{
    public class PhotoService
    {
        private readonly IWebHostEnvironment _env;
        
        public PhotoService(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            _env = env;
        }

        public string ConvertUrlToRelativePath(string url)
        {
            var uri = new Uri(url);
            var decoded = Uri.UnescapeDataString(uri.AbsolutePath);
            return decoded.TrimStart('/');
        }
        public bool DeleteFileByUrl(string url) //true - файл существует и удаляется, false - файл не найден
        {
            var relativePath = ConvertUrlToRelativePath(url).Replace("/", "\\");
            var absolutePath = Path.Combine(_env.WebRootPath, relativePath);
            if (File.Exists(absolutePath))
            { 
                File.Delete(absolutePath);
                return true;
            }
            return false;

        }

        public async Task<List<string>> SavePhotosAsync(List<IFormFile> photos, string uploadFolder)
        {
            List<string> paths = [];

            foreach (var photo in photos)
            {

                var fileName = $"{Guid.NewGuid()}_{photo.FileName}";
                
                Directory.CreateDirectory(uploadFolder);

                var filePath = Path.Combine(uploadFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await photo.CopyToAsync(stream);
                }

                paths.Add(Path.Combine("images", "food_items", fileName).Replace("\\", "/"));
            }

            return paths;
        }

        public int CountExistingPhotosByUrl(List<string>? urls)
        {
            return urls?.Select(url => File.Exists(ConvertUrlToRelativePath(url).Replace("/", "\\")) ? 1 : 0).Sum() ?? 0;
        }
        public int CountExistingPhotosByPath(List<string>? paths)
        {
            return paths?.Select(path => File.Exists(path.Replace("/", "\\")) ? 1 : 0).Sum() ?? 0;
        }
    }

}
