using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public interface IUserRolesTable<TRole, TKey, TUserRole>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
{
	Task<IEnumerable<TRole>> GetRolesAsync(TKey userId);
	Task<TUserRole> FindUserRoleAsync(TKey userId, TKey roleId);
}