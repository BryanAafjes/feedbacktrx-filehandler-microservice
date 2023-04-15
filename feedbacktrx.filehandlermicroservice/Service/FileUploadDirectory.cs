namespace feedbacktrx.filehandlermicroservice.Service
{
    public class FileUploadDirectory
    {
        public static string directoryName = "files";
        public static string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), directoryName);

        public static void Create()
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
