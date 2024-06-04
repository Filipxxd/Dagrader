using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public interface IRoleClaimsTable<TKey, TRoleClaim>
	where TKey : IEquatable<TKey>
	where TRoleClaim : IdentityRoleClaim<TKey>, new()
{
	Task<IEnumerable<TRoleClaim>> GetClaimsAsync(TKey roleId);
}
