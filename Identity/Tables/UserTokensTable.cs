using Dapper;
using Data;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public class UserTokensTable<TKey, TUserToken>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IUserTokensTable<TKey, TUserToken>
	where TKey : IEquatable<TKey>
	where TUserToken : IdentityUserToken<TKey>, new()
{
	public virtual async Task<IEnumerable<TUserToken>> GetTokensAsync(TKey userId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserTokens]
                WHERE [UserId] = @UserId;
            ";
		var userTokens = await DbConnection.QueryAsync<TUserToken>(sql, new { UserId = userId });
		return userTokens;
	}

	public virtual async Task<TUserToken> FindTokenAsync(TKey userId, string loginProvider, string name)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserTokens]
                WHERE [UserId] = @UserId AND [LoginProvider] = @LoginProvider AND [Name] = @Name;
            ";
		var token = await DbConnection.QuerySingleOrDefaultAsync<TUserToken>(sql, new
		{
			UserId = userId,
			LoginProvider = loginProvider,
			Name = name
		});
		return token;
	}
}