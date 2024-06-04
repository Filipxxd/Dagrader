using Identity.Stores;
using Identity.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace Identity;

public static class IdentityBuilderExtensions
{
	public static IdentityBuilder AddDapperStores(this IdentityBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder.RoleType);
		AddStores(builder.Services, builder.UserType, builder.RoleType);
		return builder;
	}

	private static void AddStores(IServiceCollection services, Type userType, Type roleType)
	{
		var identityUserType = FindGenericBaseType(userType, typeof(IdentityUser<>))
			?? throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a user that derives from IdentityUser<TKey>.");
		var keyType = identityUserType.GenericTypeArguments[0];
		Type userStoreType;
		var userClaimType = typeof(IdentityUserClaim<>).MakeGenericType(keyType);
		var userRoleType = typeof(IdentityUserRole<>).MakeGenericType(keyType);
		var userLoginType = typeof(IdentityUserLogin<>).MakeGenericType(keyType);
		var roleClaimType = typeof(IdentityRoleClaim<>).MakeGenericType(keyType);
		var userTokenType = typeof(IdentityUserToken<>).MakeGenericType(keyType);

		if (roleType != null)
		{
			var identityRoleType = FindGenericBaseType(roleType, typeof(IdentityRole<>))
				?? throw new InvalidOperationException($"Method {nameof(AddDapperStores)} can only be called with a role that derives from IdentityRole<TKey>.");

			services.TryAddScoped(
				typeof(IUsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType, userLoginType, userTokenType),
				typeof(UsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, userRoleType, userLoginType, userTokenType)
			);
			services.TryAddScoped(typeof(IRolesTable<,,>).MakeGenericType(roleType, keyType, roleClaimType), typeof(RolesTable<,,>).MakeGenericType(roleType, keyType, roleClaimType));
			services.TryAddScoped(typeof(IUserRolesTable<,,>).MakeGenericType(roleType, keyType, userRoleType), typeof(UserRolesTable<,,>).MakeGenericType(roleType, keyType, userRoleType));
			services.TryAddScoped(typeof(IRoleClaimsTable<,>).MakeGenericType(keyType, roleClaimType), typeof(RoleClaimsTable<,>).MakeGenericType(keyType, roleClaimType));
			services.TryAddScoped(typeof(IRoleStore<>).MakeGenericType(roleType), typeof(RoleStore<,,,>).MakeGenericType(roleType, keyType, userRoleType, roleClaimType));
			userStoreType = typeof(UserStore<,,,,,>).MakeGenericType(roleType, userClaimType, userRoleType, userLoginType, userTokenType, roleClaimType);
		}
		else
		{
			services.TryAddScoped(
				typeof(IUsersOnlyTable<,,,,>).MakeGenericType(userType, keyType, userClaimType, userLoginType, userTokenType),
				typeof(UsersTable<,,,,,>).MakeGenericType(userType, keyType, userClaimType, roleType ?? throw new NullReferenceException(), userLoginType, userTokenType)
			);
			userStoreType = typeof(UserOnlyStore<,,>).MakeGenericType(userType, keyType, userClaimType, userLoginType, userTokenType);
		}
		services.TryAddScoped(typeof(IUserClaimsTable<,>).MakeGenericType(keyType, userClaimType), typeof(UserClaimsTable<,>).MakeGenericType(keyType, userClaimType));
		services.TryAddScoped(typeof(IUserLoginsTable<,,>).MakeGenericType(userType, keyType, userLoginType), typeof(UserLoginsTable<,,>).MakeGenericType(userType, keyType, userLoginType));
		services.TryAddScoped(typeof(IUserTokensTable<,>).MakeGenericType(keyType, userTokenType), typeof(UserTokensTable<,>).MakeGenericType(keyType, userTokenType));
		services.TryAddScoped(typeof(IUserStore<>).MakeGenericType(userType), userStoreType);
	}

	private static TypeInfo? FindGenericBaseType(Type currentType, Type genericBaseType)
	{
		var type = currentType;
		while (type != null)
		{
			var typeInfo = type.GetTypeInfo();
			var genericType = type.IsGenericType ? type.GetGenericTypeDefinition() : null;
			if (genericType != null && genericType == genericBaseType)
			{
				return typeInfo;
			}
			type = type.BaseType;
		}
		return null;
	}
}