using Dapper;
using Data;
using Domain.Models;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using System.Data;
using System.Security.Claims;

namespace Identity.Tables;

public class UsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>(IDbConnectionFactory dbConnectionFactory) : DbConnectionTable(dbConnectionFactory), IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken>
	where TUser : AppUser
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>, new()
	where TUserRole : IdentityUserRole<TKey>, new()
	where TUserLogin : IdentityUserLogin<TKey>, new()
	where TUserToken : IdentityUserToken<TKey>, new()
{
	public virtual async Task<bool> CreateAsync(TUser user)
	{
		const string sql = @"
                INSERT INTO [dbo].[AspNetUsers]
                VALUES (@Id, @UserName, @NormalizedUserName, @Email, @NormalizedEmail, @EmailConfirmed, @PasswordHash, @SecurityStamp, @ConcurrencyStamp, @PhoneNumber, @PhoneNumberConfirmed, @TwoFactorEnabled, @LockoutEnd, @LockoutEnabled, @AccessFailedCount, @FirstName, @LastName, @Gender, @CourseId);
            ";
		var rowsInserted = await DbConnection.ExecuteAsync(sql, new
		{
			user.Id,
			user.UserName,
			user.NormalizedUserName,
			user.Email,
			user.NormalizedEmail,
			user.EmailConfirmed,
			user.PasswordHash,
			user.SecurityStamp,
			user.ConcurrencyStamp,
			user.PhoneNumber,
			user.PhoneNumberConfirmed,
			user.TwoFactorEnabled,
			user.LockoutEnd,
			user.LockoutEnabled,
			user.AccessFailedCount,
			user.FirstName,
			user.LastName,
			user.Gender,
			user.CourseId
		});
		return rowsInserted == 1;
	}

	public virtual async Task<bool> DeleteAsync(TKey userId)
	{
		const string sql = @"
                DELETE
                FROM [dbo].[AspNetUsers]
                WHERE [Id] = @Id;
            ";
		var rowsDeleted = await DbConnection.ExecuteAsync(sql, new { Id = userId });
		return rowsDeleted == 1;
	}

	public virtual async Task<TUser> FindByIdAsync(TKey userId)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUsers]
                WHERE [Id] = @Id;";

		var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new { Id = userId });
		return user;
	}

	public virtual async Task<TUser> FindByNameAsync(string normalizedUserName)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUsers]
                WHERE [NormalizedUserName] = @NormalizedUserName;
            ";
		var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new { NormalizedUserName = normalizedUserName });
		return user;
	}

	public virtual async Task<TUser> FindByEmailAsync(string normalizedEmail)
	{
		const string sql = @"
                SELECT * 
                FROM [dbo].[AspNetUsers]
                WHERE [NormalizedEmail] = @NormalizedEmail;
            ";
		var user = await DbConnection.QuerySingleOrDefaultAsync<TUser>(sql, new { NormalizedEmail = normalizedEmail });
		return user;
	}

	public virtual Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserLogin> logins, IList<TUserToken> tokens) => UpdateAsync(user, claims, null, logins, tokens);

	public virtual async Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserRole>? roles, IList<TUserLogin> logins, IList<TUserToken> tokens)
	{
		const string updateUserSql = @"
                UPDATE [dbo].[AspNetUsers]
                SET [UserName] = @UserName, 
                    [NormalizedUserName] = @NormalizedUserName, 
                    [Email] = @Email, 
                    [NormalizedEmail] = @NormalizedEmail, 
                    [EmailConfirmed] = @EmailConfirmed, 
                    [PasswordHash] = @PasswordHash, 
                    [SecurityStamp] = @SecurityStamp, 
                    [ConcurrencyStamp] = @ConcurrencyStamp, 
                    [PhoneNumber] = @PhoneNumber, 
                    [PhoneNumberConfirmed] = @PhoneNumberConfirmed, 
                    [TwoFactorEnabled] = @TwoFactorEnabled, 
                    [LockoutEnd] = @LockoutEnd, 
                    [LockoutEnabled] = @LockoutEnabled, 
                    [AccessFailedCount] = @AccessFailedCount,
					[FirstName] = @FirstName,
					[LastName] = @LastName,
					[Gender] = @Gender,
					[CourseId] = @CourseId
                WHERE [Id] = @Id;";

		using var transaction = DbConnection.BeginTransaction();

		await DbConnection.ExecuteAsync(updateUserSql, new
		{
			user.UserName,
			user.NormalizedUserName,
			user.Email,
			user.NormalizedEmail,
			user.EmailConfirmed,
			user.PasswordHash,
			user.SecurityStamp,
			user.ConcurrencyStamp,
			user.PhoneNumber,
			user.PhoneNumberConfirmed,
			user.TwoFactorEnabled,
			user.LockoutEnd,
			user.LockoutEnabled,
			user.AccessFailedCount,
			user.FirstName,
			user.LastName,
			user.Gender,
			user.CourseId,
			user.Id
		}, transaction);

		if (claims?.Count > 0)
		{
			const string deleteClaimsSql = @"
                        DELETE 
                        FROM [dbo].[AspNetUserClaims]
                        WHERE [UserId] = @UserId;";

			await DbConnection.ExecuteAsync(deleteClaimsSql, new { UserId = user.Id }, transaction);

			const string insertClaimsSql = @"
                        INSERT INTO [dbo].[AspNetUserClaims] (UserId, ClaimType, ClaimValue)
                        VALUES (@UserId, @ClaimType, @ClaimValue);";

			await DbConnection.ExecuteAsync(insertClaimsSql, claims.Select(x => new
			{
				UserId = user.Id,
				x.ClaimType,
				x.ClaimValue
			}), transaction);
		}

		if (roles?.Count > 0)
		{
			const string deleteRolesSql = @"
                        DELETE
                        FROM [dbo].[AspNetUserRoles]
                        WHERE [UserId] = @UserId;";

			await DbConnection.ExecuteAsync(deleteRolesSql, new { UserId = user.Id }, transaction);

			const string insertRolesSql = @"
                        INSERT INTO [dbo].[AspNetUserRoles] (UserId, RoleId)
                        VALUES (@UserId, @RoleId);";

			await DbConnection.ExecuteAsync(insertRolesSql, roles.Select(x => new
			{
				UserId = user.Id,
				x.RoleId
			}), transaction);
		}

		if (logins?.Count > 0)
		{
			const string deleteLoginsSql = @"
                        DELETE
                        FROM [dbo].[AspNetUserLogins]
                        WHERE [UserId] = @UserId;";

			await DbConnection.ExecuteAsync(deleteLoginsSql, new { UserId = user.Id }, transaction);

			const string insertLoginsSql = @"
                        INSERT INTO [dbo].[AspNetUserLogins] (LoginProvider, ProviderKey, ProviderDisplayName, UserId)
                        VALUES (@LoginProvider, @ProviderKey, @ProviderDisplayName, @UserId);";

			await DbConnection.ExecuteAsync(insertLoginsSql, logins.Select(x => new
			{
				x.LoginProvider,
				x.ProviderKey,
				x.ProviderDisplayName,
				UserId = user.Id
			}), transaction);
		}

		if (tokens?.Count > 0)
		{
			const string deleteTokensSql = @"
                        DELETE
                        FROM [dbo].[AspNetUserTokens]
                        WHERE [UserId] = @UserId;";

			await DbConnection.ExecuteAsync(deleteTokensSql, new { UserId = user.Id }, transaction);

			const string insertTokensSql = @"
                        INSERT INTO [dbo].[AspNetUserTokens] (UserId, LoginProvider, Name, Value)
                        VALUES (@UserId, @LoginProvider, @Name, @Value);";

			await DbConnection.ExecuteAsync(insertTokensSql, tokens.Select(x => new
			{
				x.UserId,
				x.LoginProvider,
				x.Name,
				x.Value
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
		return true;
	}

	public virtual async Task<IEnumerable<TUser>> GetUsersInRoleAsync(string roleName)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUsers] AS [u]
                INNER JOIN [dbo].[AspNetUserRoles] AS [ur] ON [u].[Id] = [ur].[UserId]
                INNER JOIN [dbo].[AspNetRoles] AS [r] ON [ur].[RoleId] = [r].[Id]
                WHERE [r].[Name] = @RoleName;
            ";

		return await DbConnection.QueryAsync<TUser>(sql, new { RoleName = roleName });
	}

	public virtual async Task<IEnumerable<TUser>> GetUsersForClaimAsync(Claim claim)
	{
		const string sql = @"
                SELECT *
                FROM [dbo].[AspNetUsers] AS [u]
                INNER JOIN [dbo].[AspNetUserClaims] AS [uc] ON [u].[Id] = [uc].[UserId]
                WHERE [uc].[ClaimType] = @ClaimType AND [uc].[ClaimValue] = @ClaimValue;";

		return await DbConnection.QueryAsync<TUser>(sql, new
		{
			ClaimType = claim.Type,
			ClaimValue = claim.Value
		});
	}
}