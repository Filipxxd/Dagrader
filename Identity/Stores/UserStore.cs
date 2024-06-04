#nullable disable
using Domain.Models;
using Identity.Tables;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Identity.Stores;

public class UserStore<TRole, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim> : UserStoreBase<AppUser, TRole, string, TUserClaim, TUserRole, TUserLogin, TUserToken, TRoleClaim>, IProtectedUserStore<AppUser>
	where TRole : IdentityRole<string>
	where TUserClaim : IdentityUserClaim<string>, new()
	where TUserRole : IdentityUserRole<string>, new()
	where TUserLogin : IdentityUserLogin<string>, new()
	where TUserToken : IdentityUserToken<string>, new()
	where TRoleClaim : IdentityRoleClaim<string>, new()
{

	public UserStore(IUsersTable<AppUser, string, TUserClaim, TUserRole, TUserLogin, TUserToken> usersTable, IUserClaimsTable<string, TUserClaim> userClaimsTable, IUserRolesTable<TRole, string, TUserRole> userRolesTable,
		IUserLoginsTable<AppUser, string, TUserLogin> userLoginsTable, IUserTokensTable<string, TUserToken> userTokensTable, IRolesTable<TRole, string, TRoleClaim> rolesTable, IdentityErrorDescriber describer) : base(describer)
	{
		UsersTable = usersTable ?? throw new ArgumentNullException(nameof(usersTable));
		UserClaimsTable = userClaimsTable ?? throw new ArgumentNullException(nameof(userClaimsTable));
		UserRolesTable = userRolesTable ?? throw new ArgumentNullException(nameof(userRolesTable));
		UserLoginsTable = userLoginsTable ?? throw new ArgumentNullException(nameof(userLoginsTable));
		UserTokensTable = userTokensTable ?? throw new ArgumentNullException(nameof(userTokensTable));
		RolesTable = rolesTable ?? throw new ArgumentNullException(nameof(rolesTable));
	}


	private IList<TUserClaim> UserClaims { get; set; }
	private IList<TUserRole> UserRoles { get; set; }
	private IList<TUserLogin> UserLogins { get; set; }
	private IList<TUserToken> UserTokens { get; set; }
	public IUsersTable<AppUser, string, TUserClaim, TUserRole, TUserLogin, TUserToken> UsersTable { get; }
	public IUserClaimsTable<string, TUserClaim> UserClaimsTable { get; }
	public IUserRolesTable<TRole, string, TUserRole> UserRolesTable { get; }
	public IUserLoginsTable<AppUser, string, TUserLogin> UserLoginsTable { get; }
	public IUserTokensTable<string, TUserToken> UserTokensTable { get; }
	public IRolesTable<TRole, string, TRoleClaim> RolesTable { get; }


	public override IQueryable<AppUser> Users => throw new NotSupportedException();


	public override async Task AddClaimsAsync(AppUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
	{
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		claims.ThrowIfNull(nameof(claims));
		UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
		foreach (var claim in claims)
		{
			UserClaims.Add(CreateUserClaim(user, claim));
		}
	}


	public override async Task AddLoginAsync(AppUser user, UserLoginInfo login, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		login.ThrowIfNull(nameof(login));
		UserLogins ??= (await UserLoginsTable.GetLoginsAsync(user.Id)).ToList();
		UserLogins.Add(CreateUserLogin(user, login));
	}


	public override async Task AddToRoleAsync(AppUser user, string normalizedRoleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		if (string.IsNullOrEmpty(normalizedRoleName))
		{
			throw new ArgumentException($"Parameter {nameof(normalizedRoleName)} cannot be null or empty.");
		}
		var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
		if (roleEntity == null)
		{
			throw new InvalidOperationException($"Role '{normalizedRoleName}' was not found.");
		}
		var userRoles = (await UserRolesTable.GetRolesAsync(user.Id))?.Select(x => new TUserRole
		{
			UserId = user.Id,
			RoleId = x.Id
		}).ToList();
		UserRoles = userRoles;
		UserRoles.Add(CreateUserRole(user, roleEntity));
	}


	public override async Task<IdentityResult> CreateAsync(AppUser user, CancellationToken cancellationToken = default)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		var created = await UsersTable.CreateAsync(user);
		return created ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
		{
			Code = string.Empty,
			Description = $"User '{user.UserName}' could not be created."
		});
	}


	public override async Task<IdentityResult> DeleteAsync(AppUser user, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		var deleted = await UsersTable.DeleteAsync(user.Id);
		return deleted ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
		{
			Code = string.Empty,
			Description = $"User '{user.UserName}' could not be deleted."
		});
	}


	public override async Task<AppUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var user = await UsersTable.FindByEmailAsync(normalizedEmail);
		return user;
	}


	public override async Task<AppUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var id = ConvertIdFromString(userId);
		var user = await UsersTable.FindByIdAsync(id);
		return user;
	}


	public override async Task<AppUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var user = await UsersTable.FindByNameAsync(normalizedUserName);
		return user;
	}


	public override async Task<IList<Claim>> GetClaimsAsync(AppUser user, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		var userClaims = await UserClaimsTable.GetClaimsAsync(user.Id);
		return userClaims.Select(x => new Claim(x.ClaimType, x.ClaimValue)).ToList();
	}


	public override async Task<IList<UserLoginInfo>> GetLoginsAsync(AppUser user, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		var userLogins = await UserLoginsTable.GetLoginsAsync(user.Id);
		return userLogins.Select(x => new UserLoginInfo(x.LoginProvider, x.ProviderKey, x.ProviderDisplayName)).ToList();
	}


	public override async Task<AppUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		if (string.IsNullOrEmpty(loginProvider))
		{
			throw new ArgumentNullException(nameof(loginProvider));
		}
		if (string.IsNullOrEmpty(providerKey))
		{
			throw new ArgumentNullException(nameof(providerKey));
		}
		var userLogin = await FindUserLoginAsync(loginProvider, providerKey, cancellationToken);
		if (userLogin != null)
		{
			return await FindUserAsync(userLogin.UserId, cancellationToken);
		}
		return null;
	}


	public override async Task<IList<string>> GetRolesAsync(AppUser user, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		var userRoles = await UserRolesTable.GetRolesAsync(user.Id);
		return userRoles.Select(x => x.Name).ToList();
	}


	public override async Task<IList<AppUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		claim.ThrowIfNull(nameof(claim));
		var users = await UsersTable.GetUsersForClaimAsync(claim);
		return users.ToList();
	}


	public override async Task<IList<AppUser>> GetUsersInRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		if (string.IsNullOrEmpty(normalizedRoleName))
		{
			throw new ArgumentNullException(nameof(normalizedRoleName));
		}
		var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
		var users = new List<AppUser>();
		if (role != null)
		{
			users = (await UsersTable.GetUsersInRoleAsync(normalizedRoleName)).ToList();
		}
		return users;
	}


	public override async Task<bool> IsInRoleAsync(AppUser user, string normalizedRoleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		if (string.IsNullOrEmpty(normalizedRoleName))
		{
			throw new ArgumentException(nameof(normalizedRoleName));
		}
		var role = await FindRoleAsync(normalizedRoleName, cancellationToken);
		if (role != null)
		{
			var userRole = await FindUserRoleAsync(user.Id, role.Id, cancellationToken);
			return userRole != null;
		}
		return false;
	}


	public override async Task RemoveClaimsAsync(AppUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
	{
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		claims.ThrowIfNull(nameof(claims));
		UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
		foreach (var claim in claims)
		{
			var matchedClaims = UserClaims.Where(x => x.UserId.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
			foreach (var matchedClaim in matchedClaims)
			{
				UserClaims.Remove(matchedClaim);
			}
		}
	}


	public override async Task RemoveFromRoleAsync(AppUser user, string normalizedRoleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		if (string.IsNullOrEmpty(normalizedRoleName))
		{
			throw new ArgumentException(nameof(normalizedRoleName));
		}
		var roleEntity = await FindRoleAsync(normalizedRoleName, cancellationToken);
		if (roleEntity != null)
		{
			var userRoles = (await UserRolesTable.GetRolesAsync(user.Id))?.Select(x => new TUserRole
			{
				UserId = user.Id,
				RoleId = x.Id
			}).ToList();
			UserRoles = userRoles;
			var userRole = await FindUserRoleAsync(user.Id, roleEntity.Id, cancellationToken);
			if (userRole != null)
			{
				UserRoles.Remove(userRole);
			}
		}
	}


	public override async Task RemoveLoginAsync(AppUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		UserLogins ??= (await UserLoginsTable.GetLoginsAsync(user.Id)).ToList();
		var userLogin = await FindUserLoginAsync(user.Id, loginProvider, providerKey, cancellationToken);
		if (userLogin != null)
		{
			UserLogins.Remove(userLogin);
		}
	}


	public override async Task ReplaceClaimAsync(AppUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		claim.ThrowIfNull(nameof(claim));
		newClaim.ThrowIfNull(nameof(newClaim));
		UserClaims ??= (await UserClaimsTable.GetClaimsAsync(user.Id)).ToList();
		var matchedClaims = UserClaims.Where(x => x.UserId.Equals(user.Id) && x.ClaimType == claim.Type && x.ClaimValue == claim.Value);
		foreach (var matchedClaim in matchedClaims)
		{
			matchedClaim.ClaimValue = newClaim.Value;
			matchedClaim.ClaimType = newClaim.Type;
		}
	}


	public override async Task<IdentityResult> UpdateAsync(AppUser user, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		user.ThrowIfNull(nameof(user));
		user.ConcurrencyStamp = Guid.NewGuid().ToString();
		var updated = await UsersTable.UpdateAsync(user, UserClaims, UserRoles, UserLogins, UserTokens);
		return updated ? IdentityResult.Success : IdentityResult.Failed(new IdentityError
		{
			Code = string.Empty,
			Description = $"User '{user.UserName}' could not be deleted."
		});
	}


	protected override async Task<TUserToken> FindTokenAsync(AppUser user, string loginProvider, string name, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var token = await UserTokensTable.FindTokenAsync(user.Id, loginProvider, name);
		return token;
	}


	protected override async Task<AppUser> FindUserAsync(string userId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var user = await UsersTable.FindByIdAsync(userId);
		return user;
	}


	protected override async Task<TUserLogin> FindUserLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var userLogin = await UserLoginsTable.FindUserLoginAsync(loginProvider, providerKey);
		return userLogin;
	}


	protected override async Task<TUserLogin> FindUserLoginAsync(string userId, string loginProvider, string providerKey, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var userLogin = await UserLoginsTable.FindUserLoginAsync(userId, loginProvider, providerKey);
		return userLogin;
	}


	protected override async Task AddUserTokenAsync(TUserToken token)
	{
		token.ThrowIfNull(nameof(token));
		UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
		UserTokens.Add(token);
	}


	protected override async Task RemoveUserTokenAsync(TUserToken token)
	{
		UserTokens ??= (await UserTokensTable.GetTokensAsync(token.UserId)).ToList();
		UserTokens.Remove(token);
	}


	protected override Task<TRole> FindRoleAsync(string normalizedRoleName, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var findRoleTask = RolesTable.FindByNameAsync(normalizedRoleName);
		return findRoleTask;
	}


	protected override async Task<TUserRole> FindUserRoleAsync(string userId, string roleId, CancellationToken cancellationToken)
	{
		cancellationToken.ThrowIfCancellationRequested();
		ThrowIfDisposed();
		var userRole = await UserRolesTable.FindUserRoleAsync(userId, roleId);
		return userRole;
	}
}
