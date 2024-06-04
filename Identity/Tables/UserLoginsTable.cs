using Dapper;
using Data;
using Domain.Models;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public class UserLoginsTable<TUser, TKey, TUserLogin>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IUserLoginsTable<TUser, TKey, TUserLogin>
	where TUser : AppUser
	where TKey : IEquatable<TKey>
	where TUserLogin : IdentityUserLogin<TKey>, new()
{
	public virtual async Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserLogins]
                WHERE [UserId] = @UserId;
            ";
		var userLogins = await DbConnection.QueryAsync<TUserLogin>(sql, new { UserId = userId });
		return userLogins;
	}

	public virtual async Task<TUser> FindByLoginAsync(string loginProvider, string providerKey)
	{
		const string sql = @"
                SELECT [u].*
                FROM [dbo].[AspNetUsers] AS [u]
                INNER JOIN [dbo].[AspNetUserLogins] AS [ul] ON [ul].[UserId] = [u].[Id]
                WHERE [ul].[LoginProvider] = @LoginProvider AND [ul].[ProviderKey] = @ProviderKey;
            ";
		var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new
		{
			LoginProvider = loginProvider,
			ProviderKey = providerKey
		});
		return user;
	}

	public virtual async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserLogins]
                WHERE [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;
            ";
		var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
		{
			LoginProvider = loginProvider,
			ProviderKey = providerKey
		});
		return userLogin;
	}

	public virtual async Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserLogins]
                WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [ProviderKey] = @ProviderKey;
            ";
		var userLogin = await DbConnection.QuerySingleOrDefaultAsync<TUserLogin>(sql, new
		{
			UserId = userId,
			LoginProvider = loginProvider,
			ProviderKey = providerKey
		});
		return userLogin;
	}
}