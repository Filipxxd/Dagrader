namespace Application.Auth;

public sealed record LoginCredentials(string Email = "", string Password = "", bool IsPersistant = false);
