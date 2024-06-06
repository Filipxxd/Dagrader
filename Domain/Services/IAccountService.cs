using Domain.Enums;
using Domain.Models;

namespace Domain.Services;

public interface IAccountService
{
	Task<AccountOperationResult> RegisterUser(AppUser newUser, string rawPassword, string registerURL);
	Task<AccountOperationResult> CheckLoginForUser(string email, string password);
	Task<AccountOperationResult> ConfirmEmail(string userID, string encodedToken);
	Task<AccountOperationResult> SendEmailResetPassword(string email, string passwordResetURL);
	Task<AccountOperationResult> SubmitPasswordReset(string encodedResetToken, string email, string password);
	Task<AccountOperationResult> SubmitPasswordReset(string originalPassword, string newPassword);
}
