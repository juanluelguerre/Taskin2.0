namespace ElGuerre.Taskin.Application.Exceptions;

public class ForbiddenEntityException : TaskinExceptionBase
{
    public ForbiddenEntityException(Type type, Guid id)
        : base("FORBIDDEN_ENTITY", $"Entity of type {type} with id {id} is forbidden")
    {
    }

    public ForbiddenEntityException(Type type, string code)
        : base("FORBIDDEN_ENTITY", $"Entity of type {type} with code {code} is forbidden")
    {
    }
}

public class ForbiddenEntityException<T> : ForbiddenEntityException
{
    public ForbiddenEntityException(Guid id) : base(typeof(T), id)
    {
    }

    public ForbiddenEntityException(string code) : base(typeof(T), code)
    {
    }
}