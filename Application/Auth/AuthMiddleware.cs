using Application.Constants;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace Application.Auth;

public sealed class AuthMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;
	private static readonly ConcurrentDictionary<string, LoginCredentials> PendingLogins = new();

	public const string LoginKey = "key";

	public static string EnqueueLogin(LoginCredentials loginForm)
	{
		var loginKey = Guid.NewGuid().ToString();
		PendingLogins.AddOrUpdate(loginKey, loginForm, (k, o) => loginForm);

		return loginKey;
	}

	public async Task Invoke(HttpContext context, SignInManager<AppUser> signInMgr)
	{

		if (context.Request.Path == Routes.Login && context.Request.Query.ContainsKey(LoginKey))
		{
			var key = context.Request.Query[LoginKey].ToString();

			if (PendingLogins.TryGetValue(key, out var loginInfo))
			{
				await signInMgr.PasswordSignInAsync(loginInfo.Email, loginInfo.Password, loginInfo.IsPersistant, true);

				PendingLogins.TryRemove(key, out var _);
			}

			context.Response.Redirect(Routes.Home);
			return;
		}

		if (context.Request.Path == Routes.Logout)
		{
			await signInMgr.SignOutAsync();
			context.Response.Redirect(Routes.Login);
			return;
		}

		await _next.Invoke(context);
	}
}
