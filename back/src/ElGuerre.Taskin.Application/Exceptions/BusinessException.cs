using ElGuerre.Taskin.Application.Errors;

namespace ElGuerre.Taskin.Application.Exceptions;

public class BusinessException(string code, string message, params object[] values)
    : TaskinExceptionBase(code, message, values)
{
    public BusinessException(Error error) : this(error.Code, error.Description)
    {
    }

    public BusinessException(Error error, params object[] values) : this(error.Code,
        error.Description, values)
    {
    }

    public static void ThrowIfNull(object? value, Error error)
    {
        _ = value ?? throw new BusinessException(error);
    }
}