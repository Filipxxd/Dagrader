using Dapper;
using Data;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public class UserRolesTable<TRole, TKey, TUserRole>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IUserRolesTable<TRole, TKey, TUserRole>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TUserRole : IdentityUserRole<TKey>, new()
{
	public virtual async Task<IEnumerable<TRole>> GetRolesAsync(TKey userId)
	{
		const string sql = @"
                SELECT [r].*
                FROM [dbo].[AspNetRoles] AS [r]
                INNER JOIN [dbo].[AspNetUserRoles] AS [ur] ON [ur].[RoleId] = [r].[Id]
                WHERE [ur].[UserId] = @UserId;
            ";
		var userRoles = await DbConnection.QueryAsync<TRole>(sql, new { UserId = userId });
		return userRoles;
	}

	public virtual async Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUserRoles]
                WHERE [UserId] = @UserId AND [RoleId] = @RoleId;
            ";
		var userRole = await DbConnection.QuerySingleOrDefaultAsync<TUserRole>(sql, new
		{
			UserId = userId,
			RoleId = roleId
		});
		return userRole;
	}
}