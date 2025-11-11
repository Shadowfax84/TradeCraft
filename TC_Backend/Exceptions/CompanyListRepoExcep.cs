namespace TC_Backend.Exceptions
{
    public class CompanyListRepoExcep : Exception
    {
        public CompanyListRepoExcep() { }

        public CompanyListRepoExcep(string message) : base(message) { }

        public CompanyListRepoExcep(string message, Exception innerException) : base(message, innerException) { }
    }
}