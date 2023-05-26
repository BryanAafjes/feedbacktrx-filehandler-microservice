namespace feedbacktrx.filehandlermicroservice.Exceptions
{
    public class WrongFileExtensionException : Exception
    {
        public WrongFileExtensionException() : base()
        {
        }

        public WrongFileExtensionException(string message) : base(message)
        {
        }
    }
}
