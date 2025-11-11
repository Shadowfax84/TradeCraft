namespace TC_Backend.Exceptions
{
    public class CloudinaryUploadException : Exception
    {
        public CloudinaryUploadException() { }
        public CloudinaryUploadException(string message) : base(message) { }
        public CloudinaryUploadException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class CloudinaryDeleteException : Exception
    {
        public CloudinaryDeleteException() { }
        public CloudinaryDeleteException(string message) : base(message) { }
        public CloudinaryDeleteException(string message, Exception innerException) : base(message, innerException) { }
    }
}