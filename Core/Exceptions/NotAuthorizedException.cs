namespace Core.Exceptions;

public sealed class NotAuthorizedException(string className, string methodName) : Exception($"{className}: Unauthorized attempt to run method '{methodName}'.") { }
