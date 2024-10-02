using Docentify.Domain.Common.Exceptions;

namespace Docentify.Domain.Exceptions;

public class UnauthorizedException : BaseException
{
    public UnauthorizedException()
    {
    }
    
    public UnauthorizedException(string message) : base(message)
    {
    }

    public UnauthorizedException(string message, Exception innerException) : base(message, innerException)
    {
    }
}