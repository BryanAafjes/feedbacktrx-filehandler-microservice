using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public interface IFileUploadService
    {
        public Task SaveFile(IFormFile file);
    }
}
