using Application.Auth;
using Application.Validators;
using Application.ViewModels.Base;
using Core.Handlers;
using Core.Services;
using Core.Validators;
using Core.Validators.Custom;
using Data;
using Data.Tables;
using Domain.Handlers;
using Domain.Models;
using Domain.Tables;
using Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Application;

public static class ConfigExtensions
{
	public static IServiceCollection AddDbConnectionFactory(this IServiceCollection services, string? connectionString)
	{
		ArgumentNullException.ThrowIfNull(connectionString);

		services.AddScoped<IDbConnectionFactory, DbConnectionFactory>(_ =>
		{
			return new DbConnectionFactory
			{
				ConnectionString = connectionString
			};
		});
		return services;
	}

	public static IServiceCollection AddEmailHandler(this IServiceCollection services, string? apiKey)
	{
		ArgumentNullException.ThrowIfNull(apiKey);

		services.AddHttpClient<EmailHandler>();
		services.AddScoped<IEmailSender>(provider =>
		{
			return new EmailHandler(provider.GetRequiredService<IHttpClientFactory>(), provider.GetRequiredService<ILogger<EmailHandler>>())
			{
				ApiKey = apiKey
			};
		});

		return services;
	}

	public static IServiceCollection AddIdentity(this IServiceCollection services)
	{
		services.AddAuthentication(delegate (AuthenticationOptions o)
		{
			o.DefaultScheme = IdentityConstants.ApplicationScheme;
			o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
		}).AddIdentityCookies();

		services.AddIdentityCore<AppUser>(opt =>
		{
			opt.Stores.MaxLengthForKeys = 128;
			opt.Lockout.AllowedForNewUsers = false;
			opt.Lockout.MaxFailedAccessAttempts = 6;
			opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
			opt.SignIn.RequireConfirmedAccount = true;
			opt.User.RequireUniqueEmail = true;
			opt.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
		})
			.AddSignInManager()
			.AddDefaultTokenProviders()
			.AddRoles<IdentityRole>()
			.AddDapperStores();

		services.ConfigureApplicationCookie(options =>
		{
			options.Cookie.HttpOnly = true;
			options.ExpireTimeSpan = TimeSpan.FromDays(15);
			options.SlidingExpiration = true;
		});

		services.AddScoped<AuthenticationStateProvider, IdentityStateProvider>();
		services.AddScoped<ISessionHandler, SessionHandler>();

		return services;
	}

	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.Scan(scan =>
		{
			scan.FromAssembliesOf(typeof(ActivityUnitTable))
				.AddClasses(classes => classes.Where(type => type.IsClass && type.Name.EndsWith("Table")))
				.AsImplementedInterfaces()
				.WithScopedLifetime();

			scan.FromAssembliesOf(typeof(ActivityService))
				.AddClasses(classes => classes.Where(type => type.IsClass && type.Name.EndsWith("Service")))
				.AsImplementedInterfaces()
				.WithScopedLifetime();

			scan.FromAssembliesOf(typeof(Core.Validators.ActivityUnitValidator), typeof(CzechAlphabetValidator))
				.AddClasses(classes => classes.Where(type => type.IsClass && type.Name.EndsWith("Validator")))
				.AsSelf()
				.WithScopedLifetime();
		});

		services.AddScoped<EntityValidator>();

		return services;
	}

	public static IServiceCollection AddViewModels(this IServiceCollection services)
	{
		services.Scan(scan =>
		{
			scan.FromAssembliesOf(typeof(ViewModelBase))
				.AddClasses(classes => classes.Where(type => type.IsClass && type.Name.EndsWith("ViewModel")))
				.AsSelf()
				.WithTransientLifetime();

			scan.FromAssembliesOf(typeof(ModalBaseViewModel))
				.AddClasses(classes => classes.Where(type => type.IsClass && type.Name.EndsWith("Modal")))
				.AsSelf()
				.WithScopedLifetime();

			scan.FromAssembliesOf(typeof(Application.Validators.UserEditModalValidator), typeof(UserEditModalValidator))
				.AddClasses(classes => classes.Where(type => type.IsClass && type.Name.EndsWith("Validator")))
				.AsSelf()
				.WithScopedLifetime();
		});

		return services;
	}
}