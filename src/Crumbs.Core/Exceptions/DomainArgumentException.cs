namespace Crumbs.Core.Exceptions
{
    public class DomainArgumentException : DomainException
    {
        public DomainArgumentException(string argument, string message)
            : base($"Domain exception for argument '{argument}'. Message: {message}") { }
    }
}