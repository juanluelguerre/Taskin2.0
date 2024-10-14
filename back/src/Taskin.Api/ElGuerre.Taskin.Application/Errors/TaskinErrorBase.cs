namespace ElGuerre.Taskin.Application.Errors;

public abstract class TaskinErrorBase(
    string errorCode,
    string message,
    params object[] values)
{
    public Error Error { get; } = new(errorCode, message, Values: values);

    public static implicit operator Result(TaskinErrorBase error) =>
        Result.Failure(error.Error);

    public static implicit operator Error(TaskinErrorBase error) => error.Error;
}