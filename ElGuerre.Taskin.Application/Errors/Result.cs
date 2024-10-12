namespace ElGuerre.Taskin.Application.Errors;

public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Error.None || !isSuccess && error == Error.None)
        {
            throw new ArgumentException("Invalid error", nameof(error));
        }

        this.IsSuccess = isSuccess;
        this.Error = error;
    }

    public bool IsSuccess { get; }

    public bool IsFailure => !this.IsSuccess;

    public Error Error { get; }

    public static Task<Result> TaskSuccess() => Task.FromResult(new Result(true, Error.None));
    public static Result Success() => new(true, Error.None);

    public static Result<TValue> Success<TValue>(TValue value) =>
        new(value, true, Error.None);

    public static Result Failure(Error error) => new(false, error);

    public static Result<TValue> Failure<TValue>(Error error) =>
        new(default, false, error);

    public static implicit operator Result(Error error) => Failure(error);

    public Result Match(Func<Result> success, Func<Error, Result> failure)
    {
        return this.IsSuccess ? success() : failure(this.Error);
    }
}

public class Result<TValue> : Result
{
    private readonly TValue? value;

    protected internal Result(TValue? value, bool isSuccess, Error error) : base(isSuccess, error)
    {
        this.value = value;
    }

    public TValue Value => this.IsSuccess
        ? this.value!
        : throw new InvalidOperationException("The value of a failure result can't be accessed");

    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success<TValue>(value) : Failure<TValue>(Error.NullValue);

    public static implicit operator Result<TValue>(Error error) => Failure<TValue>(error);

    public static implicit operator Result<TValue>(TaskinErrorBase errorBase) =>
        Failure<TValue>(errorBase.Error);

    public Result<TValue> Match(Func<TValue, Result<TValue>> success,
        Func<Error, Result<TValue>> failure)
    {
        return this.IsSuccess ? success(this.Value) : failure(this.Error);
    }
}