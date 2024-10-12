namespace ElGuerre.Taskin.Application.Exceptions;

public class EntityAlreadyExistsException : TaskinExceptionBase
{
    public EntityAlreadyExistsException(Type type, Guid id)
        : base("ENTITY_ALREADY_EXISTS", $"Entity of type {type} with id {id} already exists")
    {
    }

    public EntityAlreadyExistsException(Type type, string code)
        : base("ENTITY_ALREADY_EXISTS",
            $"Entity of type {type} with code {code} already exists")
    {
    }

    public EntityAlreadyExistsException(Type type, params object[] values)
        : base("ENTITY_ALREADY_EXISTS", $"Entities of type {type} already exists", values)
    {
    }
}

public class EntityAlreadyExistsException<T> : EntityAlreadyExistsException
{
    public EntityAlreadyExistsException(Guid id) : base(typeof(T), id)
    {
    }

    public EntityAlreadyExistsException(string code) : base(typeof(T), code)
    {
    }

    public EntityAlreadyExistsException(params object[] values)
        : base(typeof(T), values)
    {
    }

    public static void ThrowIfNotNull(object? result, Guid id)
    {
        if (result is null) return;

        throw new EntityAlreadyExistsException<T>(id);
    }

    public static void ThrowIfNotNull(object? result, string code)
    {
        if (result is null) return;

        throw new EntityAlreadyExistsException<T>(code);
    }

    public static void ThrowIfNotNull(object? result, object[] values)
    {
        if (result is null) return;

        throw new EntityAlreadyExistsException<T>(values);
    }
}