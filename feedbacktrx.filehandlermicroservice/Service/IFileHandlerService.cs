﻿using Microsoft.AspNetCore.Mvc;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public interface IFileHandlerService
    {
        public Task<Guid> SaveFile(IFormFile file);
        public Task<FileStreamResult> GetFileStream(string fileName);
        public string GetMimeType(string fileName);
    }
}
