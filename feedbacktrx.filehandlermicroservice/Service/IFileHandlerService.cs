using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public interface IFileHandlerService
    {
        public Task<string> SaveFile(IFormFile file);
        public Task<FileStreamResult> GetFileStream(string fileName);
        public string GetMimeType(string fileName);
    }
}
