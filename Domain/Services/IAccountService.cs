using Domain.Enums;
using Domain.Models;

namespace Domain.Services;

public interface IAccountService
{
	Task<AccountOperationResult> RegisterUser(AppUser newUser, string rawPassword, string registerURL);
	Task<AccountOperationResult> CheckLoginForUser(string email, string password);
	Task<EntityOperationResult> ConfirmEmail(string userID, string encodedToken);
	Task SendEmailResetPassword(string email, string passwordResetURL);
	Task<EntityOperationResult> SubmitPasswordReset(string encodedResetToken, string email, string password);
	Task<EntityOperationResult> SubmitPasswordReset(string originalPassword, string newPassword);
}
