using Dapper;
using Data;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public class RoleClaimsTable<TKey, TRoleClaim>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IRoleClaimsTable<TKey, TRoleClaim>
	where TKey : IEquatable<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
	public virtual async Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetRoleClaims]
                WHERE [RoleId] = @RoleId;
            ";
		var roleClaims = await DbConnection.QueryAsync<TRoleClaim>(sql, new { RoleId = roleId });
		return roleClaims;
	}
}