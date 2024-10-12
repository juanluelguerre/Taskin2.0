namespace ElGuerre.Taskin.Application.Exceptions;

public class UnauthorizedException()
    : TaskinExceptionBase("INVALID_CREDENTIALS", "Credentials are not valid");