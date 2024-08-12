namespace Newsy.Persistence.Exceptions;
public class InvalidFilterException : Exception
{
    public InvalidFilterException(string errorMessage)
        : base(errorMessage)
    {
        
    }
}
