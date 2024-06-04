using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.Tables;

public interface IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
	where TUser : AppUser
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>, new()
	where TUserLogin : IdentityUserLogin<TKey>, new()
	where TUserToken : IdentityUserToken<TKey>, new()
{
	Task<bool> CreateAsync(TUser user);
	Task<bool> DeleteAsync(TKey userId);
	Task<TUser> FindByIdAsync(TKey userId);
	Task<TUser> FindByNameAsync(string normalizedUserName);
	Task<TUser> FindByEmailAsync(string normalizedEmail);
	Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserLogin> logins, IList<TUserToken> tokens);
	Task<IEnumerable<TUser>> GetUsersForClaimAsync(Claim claim);
}

public interface IUsersTable<TUser, TKey, TUserClaim, TUserRole, TUserLogin, TUserToken> : IUsersOnlyTable<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
	where TUser : AppUser
	where TKey : IEquatable<TKey>
	where TUserClaim : IdentityUserClaim<TKey>, new()
	where TUserRole : IdentityUserRole<TKey>, new()
	where TUserLogin : IdentityUserLogin<TKey>, new()
	where TUserToken : IdentityUserToken<TKey>, new()
{
	Task<bool> UpdateAsync(TUser user, IList<TUserClaim> claims, IList<TUserRole> roles, IList<TUserLogin> logins, IList<TUserToken> tokens);
	Task<IEnumerable<TUser>> GetUsersInRoleAsync(string roleName);
}