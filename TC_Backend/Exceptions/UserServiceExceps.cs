namespace TC_Backend.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() { }
        public UserAlreadyExistsException(string message) : base(message) { }
        public UserAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class RoleNotFoundException : Exception
    {
        public RoleNotFoundException() { }
        public RoleNotFoundException(string message) : base(message) { }
        public RoleNotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UserCreationException : Exception
    {
        public UserCreationException() { }
        public UserCreationException(string message) : base(message) { }
        public UserCreationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
