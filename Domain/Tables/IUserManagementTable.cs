using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Domain.Tables;

public interface IUserManagementTable
{
	Task<IEnumerable<AppUser>> GetFilteredUsers(string searchTerm, int offset, int takeAmount);
	Task<AppUser?> GetUserWithRolesAsync(string userId);
	Task<int> GetFilteredUsersCount(string searchTerm);
	Task<IEnumerable<IdentityRole>> GetAllRoles();
	Task<EntityOperationResult> AddUserRoles(string userId, IEnumerable<string> roleIdsToAdd);
	Task<EntityOperationResult> RemoveUserRoles(string userId, IEnumerable<string> roleIdsToRemove);
}
