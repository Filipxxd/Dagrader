using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Identity.Tables;

public interface IUserLoginsTable<TUser, TKey, TUserLogin>
	where TUser : AppUser
	where TKey : IEquatable<TKey>
	where TUserLogin : IdentityUserLogin<TKey>, new()
{
	Task<IEnumerable<TUserLogin>> GetLoginsAsync(TKey userId);
	Task<TUser> FindByLoginAsync(string loginProvider, string providerKey);
	Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey);
	Task<TUserLogin> FindUserLoginAsync(TKey userId, string loginProvider, string providerKey);
}