using System.Reflection;

namespace ElGuerre.Taskin.Application.Exceptions;

public class KeyAlreadyExistsException(MemberInfo type, string keyProperty, string keyValue)
    : TaskinExceptionBase("KEY_ALREADY_EXISTS",
        $"Entity of type {type.Name} with {keyProperty} key {keyValue} already exists");

public class KeyAlreadyExistsException<T>(string keyProperty, string keyValue) :
    KeyAlreadyExistsException(typeof(T), keyProperty, keyValue);