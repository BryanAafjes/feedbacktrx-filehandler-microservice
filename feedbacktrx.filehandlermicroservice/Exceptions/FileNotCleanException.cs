namespace feedbacktrx.filehandlermicroservice.Exceptions
{
    public class FileNotCleanException : Exception
    {
        public FileNotCleanException() : base()
        {
        }

        public FileNotCleanException(string message) : base(message)
        {
        }
    }
}
