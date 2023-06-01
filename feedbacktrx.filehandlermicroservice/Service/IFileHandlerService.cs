using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public interface IFileHandlerService
    {
        public Task<string> SaveFile(IFormFile file);
        public Task<FileStreamResult> GetFileStream(string fileName);
        public Task<bool> DeleteFileFromBlobStorage(string fileName);
        public Task<bool> CheckIfFileExists(string fileName);
        public string GetMimeType(string fileName);
    }
}
