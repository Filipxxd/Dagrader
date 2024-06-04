using Dapper;
using Domain.Enums;
using Domain.Models;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Data.Tables;

public sealed class UserManagementTable(IDbConnectionFactory factory, ILogger<UserManagementTable> logger) : DbConnectionTable(factory), IUserManagementTable
{
	private readonly ILogger<UserManagementTable> _logger = logger;

	public async Task<IEnumerable<AppUser>> GetFilteredUsers(string searchTerm, int offset, int takeAmount)
	{
		const string sql = @"
            WITH UserRoles AS (
                SELECT u.Id, u.FirstName, r.Name as RoleName
                FROM AspNetUsers u
                LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
                LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
                WHERE u.EmailConfirmed = 1 
                AND (u.FirstName LIKE @SearchTerm 
                OR u.LastName LIKE @SearchTerm 
                OR CONCAT(u.FirstName, ' ', u.LastName) LIKE @SearchTerm 
                OR r.Name LIKE @SearchTerm)
            ),
            UserPagination AS (
                SELECT Id
                FROM UserRoles
                GROUP BY Id
                ORDER BY MIN(FirstName) ASC
                OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY
            )
            SELECT u.*, r.*
            FROM UserPagination UP
            JOIN AspNetUsers u ON u.Id = UP.Id
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId;";

		var parameters = new
		{
			SearchTerm = $"%{searchTerm}%",
			Skip = offset,
			Take = takeAmount
		};

		var userDictionary = new Dictionary<string, AppUser>();

		try
		{
			await DbConnection.QueryAsync<AppUser, IdentityRole, AppUser>(
				sql,
				(user, role) =>
				{
					if (!userDictionary.TryGetValue(user.Id, out var currentUser))
					{
						currentUser = user;
						currentUser.Roles = [];
						userDictionary.Add(currentUser.Id, currentUser);
					}

					if (role != null && !currentUser.Roles.Any(r => r.Id == role.Id))
					{
						currentUser.Roles.Add(role);
					}

					return currentUser;
				},
				param: parameters,
				splitOn: "Id"
			);

			return userDictionary.Values;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while searching for users. SearchTerm: '{SearchTerm}', Offset: '{Offset}', Take: '{TakeAmount}'", searchTerm, offset, takeAmount);
			return [];
		}
	}

	public async Task<int> GetFilteredUsersCount(string searchTerm)
	{
		const string sql = @"
            SELECT COUNT(DISTINCT u.Id) 
            FROM AspNetUsers u
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
            LEFT JOIN AspNetRoles r ON r.Id = ur.RoleId
            WHERE (u.FirstName LIKE @SearchTerm 
            OR u.LastName LIKE @SearchTerm 
            OR CONCAT(u.FirstName, ' ', u.LastName) LIKE @SearchTerm 
            OR r.Name LIKE @SearchTerm) 
            AND u.EmailConfirmed = 1";

		try
		{
			return await DbConnection.ExecuteScalarAsync<int>(sql, new { SearchTerm = $"%{searchTerm}%" });
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while counting filtered users. SearchTerm: '{SearchTerm}'", searchTerm);
			return 0;
		}
	}

	public async Task<IEnumerable<IdentityRole>> GetAllRoles()
	{
		const string sql = "SELECT * FROM AspNetRoles";

		try
		{
			return await DbConnection.QueryAsync<IdentityRole>(sql);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while retrieving all roles.");
			return [];
		}
	}

	public async Task<AppUser?> GetUserWithRolesAsync(string userId)
	{
		const string sql = @"
            SELECT u.*, r.* 
            FROM AspNetUsers u 
            LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId 
            LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
            WHERE u.Id = @UserId;";

		var parameters = new { UserId = userId };
		var userDictionary = new Dictionary<string, AppUser>();

		try
		{
			await DbConnection.QueryAsync<AppUser, IdentityRole, AppUser>(
				sql,
				(user, role) =>
				{
					if (!userDictionary.TryGetValue(user.Id, out var currentUser))
					{
						currentUser = user;
						currentUser.Roles = [];
						userDictionary[user.Id] = currentUser;
					}

					if (role?.Id != null)
					{
						currentUser.Roles.Add(role);
					}

					return currentUser;
				},
				parameters,
				splitOn: "Id"
			);

			return userDictionary.Values.FirstOrDefault();
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while getting user with roles. UserId: '{UserId}'", userId);
			return null;
		}
	}

	public async Task<EntityOperationResult> AddUserRoles(string userId, IEnumerable<string> roleIdsToAdd)
	{
		using var transaction = DbConnection.BeginTransaction();
		const string sql = "INSERT INTO AspNetUserRoles VALUES (@UserId, @RoleId);";

		try
		{
			foreach (var roleId in roleIdsToAdd)
			{
				await DbConnection.ExecuteAsync(sql, new { UserId = userId, RoleId = roleId }, transaction);
			}

			transaction.Commit();
			return EntityOperationResult.Success;
		}
		catch (SqlException ex)
		{
			_logger.LogError(ex, "An SQL error occurred while adding roles for user. UserId: '{UserId}'", userId);
			transaction.Rollback();

			return ex.Number switch
			{
				2627 or 2601 => EntityOperationResult.EntityNotValid,
				_ => EntityOperationResult.InternalServerError
			};
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while adding roles for user. UserId: '{UserId}'", userId);
			transaction.Rollback();

			return EntityOperationResult.InternalServerError;
		}
	}

	public async Task<EntityOperationResult> RemoveUserRoles(string userId, IEnumerable<string> roleIdsToRemove)
	{
		using var transaction = DbConnection.BeginTransaction();
		const string sql = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId AND RoleId = @RoleId;";

		try
		{
			foreach (var roleId in roleIdsToRemove)
			{
				await DbConnection.ExecuteAsync(sql, new { UserId = userId, RoleId = roleId }, transaction);
			}

			transaction.Commit();
			return EntityOperationResult.Success;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while removing roles for user. UserId: '{UserId}'", userId);
			transaction.Rollback();

			return EntityOperationResult.InternalServerError;
		}
	}
}
