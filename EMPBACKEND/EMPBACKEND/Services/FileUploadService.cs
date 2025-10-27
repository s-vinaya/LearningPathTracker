namespace EMPBACKEND.Services
{
    public interface IFileUploadService
    {
        Task<string> UploadVideoAsync(IFormFile file);
        Task<string> UploadDocumentAsync(IFormFile file);
        Task<bool> DeleteFileAsync(string filePath);
    }

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string[] _allowedVideoExtensions = { ".mp4", ".avi", ".mov", ".wmv", ".flv" };
        private readonly string[] _allowedDocumentExtensions = { ".pdf", ".doc", ".docx", ".ppt", ".pptx" };
        private const long MaxFileSize = 100 * 1024 * 1024; // 100MB

        public FileUploadService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> UploadVideoAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File size exceeds maximum limit");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedVideoExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "videos");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/videos/{fileName}";
        }

        public async Task<string> UploadDocumentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided");

            if (file.Length > MaxFileSize)
                throw new ArgumentException("File size exceeds maximum limit");

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!_allowedDocumentExtensions.Contains(extension))
                throw new ArgumentException("Invalid file type");

            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "documents");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/documents/{fileName}";
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}