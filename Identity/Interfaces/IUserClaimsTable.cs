using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public interface IUserClaimsTable<TKey, TUserClaim>
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>, new()
{
	Task<IEnumerable<TUserClaim>> GetClaimsAsync(TKey userId);
}