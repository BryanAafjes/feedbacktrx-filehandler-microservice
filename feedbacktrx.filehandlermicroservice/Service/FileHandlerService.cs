using feedbacktrx.filehandlermicroservice.Exceptions;

namespace feedbacktrx.filehandlermicroservice.Service
{
    public class FileHandlerService : IFileHandlerService
    {
        public async Task<Guid> SaveFile(IFormFile file)
        {
            if (file == null)
            {
                throw new FileNullException();
            }

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


        public Stream GetFileStream(string fileName)
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

            return new FileStream(audioFilePath, FileMode.Open, FileAccess.Read);
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
    }
}
