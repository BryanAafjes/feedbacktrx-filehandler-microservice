using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public class FileUploadService : IFileUploadService
    {
        public async Task SaveFile(IFormFile file)
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "files", file.FileName);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
    }
}
