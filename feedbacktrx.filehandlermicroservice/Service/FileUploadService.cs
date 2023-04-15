using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public class FileUploadService : IFileUploadService
    {
        public async Task<Guid> SaveFile(IFormFile file)
        {
            Guid guid = Guid.NewGuid();
            string fileName = guid.ToString() + Path.GetExtension(file.FileName);

            string path = Path.Combine(
                Directory.GetCurrentDirectory(), 
                FileUploadDirectory.directoryName, 
                fileName
            );

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return guid;
        }
    }
}
