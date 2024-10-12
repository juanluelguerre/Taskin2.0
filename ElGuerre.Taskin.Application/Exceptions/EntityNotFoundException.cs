namespace ElGuerre.Taskin.Application.Exceptions;

public class EntityNotFoundException : TaskinExceptionBase
{
    public EntityNotFoundException(Type type, Guid id)
        : base("ENTITY_NOT_FOUND", $"Entity of type {type} with id {id} not found")
    {
    }

    public EntityNotFoundException(Type type, string code)
        : base("ENTITY_NOT_FOUND", $"Entity of type {type} with code {code} not found")
    {
    }
}

public class EntityNotFoundException<T> : EntityNotFoundException
{
    public EntityNotFoundException(Guid id) : base(typeof(T), id)
    {
    }

    public EntityNotFoundException(string code) : base(typeof(T), code)
    {
    }

    public static TParam ThrowIfNull<TParam>(TParam result, Guid id)
    {
        return result ?? throw new EntityNotFoundException<T>(id);
    }

    public static TParam ThrowIfNull<TParam>(TParam result, string code)
    {
        return result ?? throw new EntityNotFoundException<T>(code);
    }
}