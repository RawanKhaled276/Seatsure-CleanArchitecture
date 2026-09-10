using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Seatsure.Application.Exceptions;
public abstract class AppException : Exception
{
   
    public abstract int StatusCode { get; }

   
    protected AppException(string message): base(message ){}
}
public sealed class NotFoundException : AppException
{
    public override int StatusCode => 404;
    public NotFoundException(string message) : base(message) { }
}
public sealed class ValidationException : AppException
{
    public ValidationException(string message) : base(message){}

    public override int StatusCode => 400;

}

public sealed class ConflictException : AppException
{
    public ConflictException(string message) : base(message) { }
    public override int StatusCode => 409;
}
public sealed class UnauthorizedException : AppException
{
    public UnauthorizedException(string message) : base(message) { }
    public override int StatusCode => 401;
}

public sealed class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message) { }
    public override int StatusCode => 403;
}

