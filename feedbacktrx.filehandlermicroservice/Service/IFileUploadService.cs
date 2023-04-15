using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public interface IFileUploadService
    {
        public Task<Guid> SaveFile(IFormFile file);
    }
}
