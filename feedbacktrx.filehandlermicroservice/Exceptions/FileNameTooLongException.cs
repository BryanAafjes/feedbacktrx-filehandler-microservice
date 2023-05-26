namespace feedbacktrx.filehandlermicroservice.Exceptions
{
    public class FileNameTooLongException : Exception
    {
        public FileNameTooLongException() : base()
        {
        }

        public FileNameTooLongException(string message) : base(message)
        {
        }
    }
}
