namespace ElGuerre.Taskin.Application.Exceptions;

public interface ITaskinException
{
    string Code { get; }
    string Message { get; }
    IReadOnlyCollection<object> Values { get; }
}