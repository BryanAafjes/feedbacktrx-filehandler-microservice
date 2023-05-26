using feedbacktrx.filehandlermicroservice.Exceptions;
using Microsoft.AspNetCore.Mvc;
using nClam;
using System.IO;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public class FileHandlerService : IFileHandlerService
    {
        private IClamAVService _clamAVService;

        public FileHandlerService(IClamAVService clamAVService) 
        {
            _clamAVService = clamAVService;
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            ValidateFile(file, new string[] { ".wav", ".mp3" });

            Guid guid = Guid.NewGuid();
            string fileName = guid.ToString() + Path.GetExtension(file.FileName);

            string path = Path.Combine(
                Directory.GetCurrentDirectory(), 
                FileUploadDirectory.directoryName, 
                fileName
            );

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                var scanResult = await _clamAVService.ScanFileAsync(stream);

                if (!scanResult.Result.Equals(ClamScanResults.Clean))
                {
                    throw new FileNotCleanException("File is not clean!");
                }

                await file.CopyToAsync(stream);
            }

            return fileName;
        }


        public async Task<FileStreamResult> GetFileStream(string fileName)
        {
            var audioFilePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                FileUploadDirectory.directoryName,
                fileName
            );

            if (!File.Exists(audioFilePath))
            {
                throw new Exceptions.FileNotFoundException();
            }

            FileStream fileStream = new(audioFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
            FileStreamResult result = new(fileStream, GetMimeType(fileName))
            {
                EnableRangeProcessing = true
            };

            return await Task.FromResult(result);
        }

        public string GetMimeType(string fileName)
        {
            var fileExtension = Path.GetExtension(fileName);

            switch (fileExtension.ToLower())
            {
                case ".mp3":
                    return "audio/mpeg";
                case ".wav":
                    return "audio/wav";
                default:
                    return "application/octet-stream";
            }
        }
        private static void ValidateFile(IFormFile file, string[] fileTypes)
        {
            if (file == null || file.Length == 0)
            {
                throw new FileNullException("A file hasn't been selected!");
            }

            var uploadedFileName = Path.GetFileName(file.FileName);
            var fileNameLength = uploadedFileName.Length;

            if (fileNameLength > 100)
            {
                throw new FileNameTooLongException("File name is too long!");
            }

            var allowedextensions = fileTypes;
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedextensions.Contains(fileExtension))
            {
                throw new WrongFileExtensionException("Only " + string.Join(", ", allowedextensions));
            }
        }

    }
}
