namespace POSHWeb.Exceptions
{
    public class FileStillExistException : Exception
    {
        public FileStillExistException(string message)
            : base(message)
        {
        }

    }
}

