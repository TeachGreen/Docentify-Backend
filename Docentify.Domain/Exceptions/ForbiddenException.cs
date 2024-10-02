using Docentify.Domain.Common.Exceptions;

namespace Docentify.Domain.Exceptions;

public class ForbiddenException : BaseException
{
    public ForbiddenException()
    {
    }

    public ForbiddenException(string message) : base(message)
    {
    }

    public ForbiddenException(string message, Exception innerException) : base(message, innerException)
    {
    }
}