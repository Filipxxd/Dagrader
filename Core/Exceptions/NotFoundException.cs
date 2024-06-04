namespace Core.Exceptions;

public sealed class NotFoundException(string className, string id) : Exception($"{className}: Item with id '{id} was not found.'") { }
