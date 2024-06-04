using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public interface IRolesTable<TRole, TKey, TRoleClaim>
	where TRole : IdentityRole<TKey>
	where TKey : IEquatable<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
	Task<bool> CreateAsync(TRole role);
	Task<bool> DeleteAsync(TKey roleId);
	Task<TRole> FindByIdAsync(TKey roleId);
	Task<TRole> FindByNameAsync(string normalizedName);
	Task<bool> UpdateAsync(TRole role, IList<TRoleClaim>? claims = null);
}