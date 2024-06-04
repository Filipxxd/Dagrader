namespace Domain.Handlers;

public interface ISessionHandler
{
	Task<bool> IsUserAuthenticatedAsync();
	Task<bool> IsUserInRoleAsync(string roleName);
	Task<string> GetLoggedInUserId();
}
