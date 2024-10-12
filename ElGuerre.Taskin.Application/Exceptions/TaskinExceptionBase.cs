namespace ElGuerre.Taskin.Application.Exceptions;

public abstract class TaskinExceptionBase(
    string exceptionCode,
    string message,
    params object[] values)
    : Exception(message), ITaskinException
{
    public string Code { get; protected set; } = exceptionCode;
    public IReadOnlyCollection<object> Values { get; protected set; } = values;
}