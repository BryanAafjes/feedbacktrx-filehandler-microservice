namespace feedbacktrx.filehandlermicroservice.Exceptions
{
    public class FileNotFoundException : Exception
    {
        public FileNotFoundException() : base()
        {
        }

        public FileNotFoundException(string message) : base(message)
        {
        }
    }
}
