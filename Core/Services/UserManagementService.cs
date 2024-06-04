using Domain;
using Domain.Enums;
using Domain.Handlers;
using Domain.Models;
using Domain.Pagination;
using Domain.Services;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public sealed class UserManagementService(ISessionHandler sessionHandler, UserManager<AppUser> userManager, IUserManagementTable table, ILogger<UserManagementService> logger) : IUserManagementService
{
	private readonly ISessionHandler _sessionHandler = sessionHandler;
	private readonly UserManager<AppUser> _userManager = userManager;
	private readonly IUserManagementTable _table = table;
	private readonly ILogger<UserManagementService> _logger = logger;

	public async Task<PaginatedList<AppUser>> GetPaginatedUsersAsync(string searchTerm, int page, int itemsPerPage)
	{
		if (!await _sessionHandler.IsUserInRoleAsync(Roles.Admin))
		{
			return [];
		}

		searchTerm = searchTerm.Trim();

		var totalItems = await _table.GetFilteredUsersCount(searchTerm);
		var items = await _table.GetFilteredUsers(searchTerm, (page - 1) * itemsPerPage, itemsPerPage);

		return new(items, totalItems, page);
	}

	public async Task<IEnumerable<IdentityRole>> GetAllRolesAsync()
	{
		if (!await _sessionHandler.IsUserInRoleAsync(Roles.Admin))
		{
			return [];
		}

		return await _table.GetAllRoles();
	}

	public async Task<AppUser?> GetUserWithRolesAsync(string userId)
	{
		if (!await _sessionHandler.IsUserInRoleAsync(Roles.Admin))
		{
			return null;
		}

		return await _table.GetUserWithRolesAsync(userId);
	}

	public async Task<EntityOperationResult> UpdateUser(AppUser updatedUser, IEnumerable<string> assignedRoleIds)
	{
		if (!await _sessionHandler.IsUserInRoleAsync(Roles.Admin))
		{
			return EntityOperationResult.NotAuthorized;
		}

		var user = await _table.GetUserWithRolesAsync(updatedUser.Id);

		if (user is null)
		{
			return EntityOperationResult.NotFound;
		}

		user.FirstName = updatedUser.FirstName;
		user.LastName = updatedUser.LastName;
		user.Gender = updatedUser.Gender;

		var identityResult = await _userManager.UpdateAsync(user);

		if (!identityResult.Succeeded)
		{
			_logger.LogError("Error while updating user information for userId: '{userId}'.", user.Id);
			return EntityOperationResult.InternalServerError;
		}

		var currentRoleIds = user.Roles.Select(r => r.Id).ToList();
		var roleIdsToBeAdded = assignedRoleIds.Except(currentRoleIds).ToList();
		var roleIdsToBeRemoved = currentRoleIds.Except(assignedRoleIds).ToList();

		if (roleIdsToBeAdded.Count > 0)
		{
			var result = await _table.AddUserRoles(user.Id, roleIdsToBeAdded);

			if (result != EntityOperationResult.Success)
			{
				_logger.LogError("Error while updating user roles for userId: '{userId}'.", user.Id);
				return EntityOperationResult.InternalServerError;
			}
		}

		if (roleIdsToBeRemoved.Count > 0)
		{
			var result = await _table.RemoveUserRoles(user.Id, roleIdsToBeRemoved);

			if (result != EntityOperationResult.Success)
			{
				_logger.LogError("Error while updating user roles for userId: '{userId}'.", user.Id);
				return EntityOperationResult.InternalServerError;
			}
		}
		if (roleIdsToBeAdded.Count > 0 || roleIdsToBeRemoved.Count > 0)
		{
			identityResult = await _userManager.UpdateSecurityStampAsync(user);

			if (!identityResult.Succeeded)
			{
				_logger.LogError("Error while updating the security stamp for userId: '{userId}'.", user.Id);
			}
		}

		return EntityOperationResult.Success;
	}

	public async Task<EntityOperationResult> DeleteUserAsync(string userId)
	{
		if (!await _sessionHandler.IsUserInRoleAsync(Roles.Admin))
		{
			return EntityOperationResult.NotAuthorized;
		}

		throw new NotImplementedException();
	}
}
