using Dapper;
using Data;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public class UserClaimsTable<TKey, TUserClaim>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IUserClaimsTable<TKey, TUserClaim>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>, new()
{
	public virtual async Task<IEnumerable<TUserClaim>> GetClaimsAsync(TKey userId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserClaims]
                WHERE [UserId] = @UserId;
            ";
		var userClaims = await DbConnection.QueryAsync<TUserClaim>(sql, new { UserId = userId });
		return userClaims;
	}
}