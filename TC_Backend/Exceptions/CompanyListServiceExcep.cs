namespace TC_Backend.Exceptions
{
    public class CompanyListServiceExcep : Exception
    {
        public CompanyListServiceExcep() { }

        public CompanyListServiceExcep(string message) : base(message) { }

        public CompanyListServiceExcep(string message, Exception innerException) : base(message, innerException) { }
    }
}