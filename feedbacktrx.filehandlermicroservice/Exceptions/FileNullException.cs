namespace feedbacktrx.filehandlermicroservice.Exceptions
{
    public class FileNullException : Exception
    {
        public FileNullException() : base()
        {
        }

        public FileNullException(string message) : base(message)
        {
        }
    }
}
