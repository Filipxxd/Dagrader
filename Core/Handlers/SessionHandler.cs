using Domain.Handlers;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Core.Handlers;

public sealed class SessionHandler(AuthenticationStateProvider provider) : ISessionHandler
{
	private readonly AuthenticationStateProvider _authenticationStateProvider = provider;

	public async Task<bool> IsUserAuthenticatedAsync()
	{
		return (await _authenticationStateProvider.GetAuthenticationStateAsync()).User.Identity?.IsAuthenticated ?? false;
	}

	public async Task<string> GetLoggedInUserId()
	{
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

		return authState.User.FindFirst(c => c.Type.Contains("nameidentifier"))?.Value ?? string.Empty;
	}

	public async Task<bool> IsUserInRoleAsync(string roleName)
	{
		var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();

		return authState.User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(rc => rc.Value).Any(rn => rn.Equals(roleName, StringComparison.OrdinalIgnoreCase));
	}
}
