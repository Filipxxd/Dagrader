using Application.Constants;
using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Collections.Concurrent;

namespace Application.Auth;

public sealed class AuthenticationMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;
	private static readonly ConcurrentDictionary<string, LoginCredentials> Logins = new();

	public static void Add(string key, LoginCredentials loginForm)
	{
		Logins.AddOrUpdate(key, loginForm, (k, o) => loginForm);
	}

	public async Task Invoke(HttpContext context, SignInManager<AppUser> signInMgr)
	{

		if (context.Request.Path == Routes.Login && context.Request.Query.ContainsKey("key"))
		{
			var key = context.Request.Query["key"].ToString();

			if (Logins.TryGetValue(key, out var loginInfo))
			{
				await signInMgr.PasswordSignInAsync(loginInfo.Email, loginInfo.Password, loginInfo.IsPersistant, true);

				Logins.TryRemove(key, out var _);
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
