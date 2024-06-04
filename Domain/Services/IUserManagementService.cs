using Domain.Enums;
using Domain.Models;
using Domain.Pagination;
using Microsoft.AspNetCore.Identity;

namespace Domain.Services;

public interface IUserManagementService
{
	Task<PaginatedList<AppUser>> GetPaginatedUsersAsync(string searchTerm, int page, int itemsPerPage = 20);
	Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
	Task<AppUser?> GetUserWithRolesAsync(string userId);
	Task<EntityOperationResult> UpdateUser(AppUser updatedUser, IEnumerable<string> assignedRoleIds);
	Task<EntityOperationResult> DeleteUserAsync(string userId);
}
