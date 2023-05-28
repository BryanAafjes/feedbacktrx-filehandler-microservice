using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using feedbacktrx.filehandlermicroservice.Exceptions;
using Microsoft.AspNetCore.Mvc;
using nClam;
using System.ComponentModel;
using System.IO;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public class FileHandlerService : IFileHandlerService
    {
        private IClamAVService _clamAVService;
        private IConfiguration _configuration;

        public FileHandlerService(IClamAVService clamAVService, IConfiguration configuration) 
        {
            _clamAVService = clamAVService;
            _configuration = configuration;
        }

        public async Task<string> SaveFile(IFormFile file)
        {
            ValidateFile(file, new string[] { ".wav", ".mp3" });

            Guid guid = Guid.NewGuid();
            string fileName = guid.ToString() + Path.GetExtension(file.FileName);

            using (Stream stream = file.OpenReadStream())
            {
                var scanResult = await _clamAVService.ScanFileAsync(stream);

                if (!scanResult.Equals(ClamScanResults.Clean))
                {
                    throw new FileNotCleanException("File is not clean!");
                }

                string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
                string containerName = _configuration["AzureBlobStorage:ContainerName"];
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                
                await containerClient.CreateIfNotExistsAsync();

                BlobClient blobClient = containerClient.GetBlobClient(fileName);
                stream.Position = 0;

                await blobClient.UploadAsync(stream, overwrite: true);
            }

            return fileName;
        }


        public async Task<FileStreamResult> GetFileStream(string fileName)
        {
            string connectionString = _configuration["AzureBlobStorage:ConnectionString"];
            string containerName = _configuration["AzureBlobStorage:ContainerName"];

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            if (!await blobClient.ExistsAsync())
            {
                throw new Exceptions.FileNotFoundException();
            }

            Stream stream = await blobClient.OpenReadAsync();

            FileStreamResult result = new FileStreamResult(stream, GetMimeType(fileName))
            {
                EnableRangeProcessing = true
            };

            return result;
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
