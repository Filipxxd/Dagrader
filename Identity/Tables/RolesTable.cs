using Dapper;
using Data;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace Identity.Tables;

public class RolesTable<TRole, TKey, TRoleClaim>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IRolesTable<TRole, TKey, TRoleClaim>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
	public virtual async Task<bool> CreateAsync(TRole role)
	{
		const string sql = @"
                INSERT INTO [dbo].[AspNetRoles]
                VALUES (@Id, @Name, @NormalizedName, @ConcurrencyStamp);
            ";
		var rowsInserted = await DbConnection.ExecuteAsync(sql, new
		{
			role.Id,
			role.Name,
			role.NormalizedName,
			role.ConcurrencyStamp
		});
		return rowsInserted == 1;
	}

	public virtual async Task<bool> DeleteAsync(TKey roleId)
	{
		const string sql = @"
                DELETE
                FROM [dbo].[AspNetRoles]
                WHERE [Id] = @Id;
            ";
		var rowsDeleted = await DbConnection.ExecuteAsync(sql, new { Id = roleId });
		return rowsDeleted == 1;
	}

	public virtual async Task<TRole> FindByIdAsync(TKey roleId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetRoles]
                WHERE [Id] = @Id;
            ";
		var role = await DbConnection.QuerySingleOrDefaultAsync<TRole>(sql, new { Id = roleId });
		return role;
	}

	public virtual async Task<TRole> FindByNameAsync(string normalizedName)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetRoles]
                WHERE [NormalizedName] = @NormalizedName;
            ";
		var role = await DbConnection.QuerySingleOrDefaultAsync<TRole>(sql, new { NormalizedName = normalizedName });
		return role;
	}

	public virtual async Task<bool> UpdateAsync(TRole role, IList<TRoleClaim>? claims = null)
	{
		const string updateRoleSql = @"
                UPDATE [dbo].[AspNetRoles]
                SET [Name] = @Name, [NormalizedName] = @NormalizedName, [ConcurrencyStamp] = @ConcurrencyStamp
                WHERE [Id] = @Id;
            ";
		using (var transaction = DbConnection.BeginTransaction())
		{
			await DbConnection.ExecuteAsync(updateRoleSql, new
			{
				role.Name,
				role.NormalizedName,
				role.ConcurrencyStamp,
				role.Id
			}, transaction);
			if (claims?.Count > 0)
			{
				const string deleteClaimsSql = @"
                        DELETE
                        FROM [dbo].[AspNetRoleClaims]
                        WHERE [RoleId] = @RoleId;
                    ";
				await DbConnection.ExecuteAsync(deleteClaimsSql, new
				{
					RoleId = role.Id
				}, transaction);
				const string insertClaimsSql = @"
                        INSERT INTO [dbo].[AspNetRoleClaims] (RoleId, ClaimType, ClaimValue)
                        VALUES (@RoleId, @ClaimType, @ClaimValue);
                    ";
				await DbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new
				{
					RoleId = role.Id,
					x.ClaimType,
					x.ClaimValue
				}), transaction);
			}
			try
			{
				transaction.Commit();
			}
			catch
			{
				transaction.Rollback();
				return false;
			}
		}
		return true;
	}
}