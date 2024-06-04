using Domain.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Application.Auth;

public class IdentityStateProvider(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory, IOptions<IdentityOptions> optionsAccessor)
				: RevalidatingServerAuthenticationStateProvider(loggerFactory)
{
	private readonly IServiceScopeFactory _scopeFactory = scopeFactory;
	private readonly IdentityOptions _options = optionsAccessor.Value;

	protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(1);

	protected override async Task<bool> ValidateAuthenticationStateAsync(AuthenticationState authenticationState, CancellationToken cancellationToken)
	{
		// Make sure fresh data
		var scope = _scopeFactory.CreateScope();
		try
		{
			var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
			return await ValidateSecurityStampAsync(userManager, authenticationState.User);
		}
		finally
		{
			if (scope is IAsyncDisposable asyncDisposable)
			{
				await asyncDisposable.DisposeAsync();
			}
			else
			{
				scope.Dispose();
			}
		}
	}

	private async Task<bool> ValidateSecurityStampAsync(UserManager<AppUser> userManager, ClaimsPrincipal principal)
	{
		var user = await userManager.GetUserAsync(principal);
		if (user is null)
		{
			return false;
		}

		if (!userManager.SupportsUserSecurityStamp)
		{
			return true;
		}

		var principalStamp = principal.FindFirstValue(_options.ClaimsIdentity.SecurityStampClaimType);
		var userStamp = await userManager.GetSecurityStampAsync(user);

		return principalStamp == userStamp;
	}
}