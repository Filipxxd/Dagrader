using Domain.Enums;
using Domain.Models;
using Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Encodings.Web;

namespace Core.Services;

public sealed class AccountService(UserManager<AppUser> userManager, IUserStore<AppUser> userStore, IEmailSender emailSender, SignInManager<AppUser> signInManager, ILogger<AccountService> logger) : IAccountService
{
	private readonly UserManager<AppUser> _userManager = userManager;
	private readonly IUserStore<AppUser> _userStore = userStore;
	private readonly IEmailSender _emailSender = emailSender;
	private readonly SignInManager<AppUser> _signInManager = signInManager;
	private readonly IUserEmailStore<AppUser> _emailStore = (IUserEmailStore<AppUser>)userStore;
	private readonly ILogger<AccountService> _logger = logger;

	public async Task<AccountOperationResult> RegisterUser(AppUser newUser, string rawPassword, string registerURL)
	{
		// TODO: Validate newUser

		await _userStore.SetUserNameAsync(newUser, newUser.Email, CancellationToken.None);
		await _emailStore.SetEmailAsync(newUser, newUser.Email, CancellationToken.None);

		var result = await _userManager.CreateAsync(newUser, rawPassword);

		if (!result.Succeeded)
		{
			var errorCodes = result.Errors.Select(err => err.Code);

			_logger.LogError("Unable to register new user. Errors: {ErrorCodes}", errorCodes);

			if (errorCodes.Contains("DuplicateEmail"))
			{
				return AccountOperationResult.EmailTaken;
			}

			return AccountOperationResult.NotValid;
		}

		await SendEmailConfirmation(newUser, registerURL);

		return AccountOperationResult.Success;
	}


	public async Task<AccountOperationResult> CheckLoginForUser(string email, string password)
	{
		// Validation


		var user = await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			return AccountOperationResult.NotValid;
		}

		if (!user.EmailConfirmed)
		{
			return AccountOperationResult.EmailNotConfirmed;
		}

		var canSignIn = await _signInManager.CanSignInAsync(user);
		if (!canSignIn)
		{
			return AccountOperationResult.LockedOut;
		}

		var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
		if (result.Succeeded)
		{
			return AccountOperationResult.Success;
		}

		return AccountOperationResult.NotValid;
	}


	public async Task<AccountOperationResult> SendEmailResetPassword(string email, string passwordResetURL)
	{
		// TODO: Validate email

		var user = await _userManager.FindByEmailAsync(email);

		if (user is null)
		{
			return AccountOperationResult.NotValid;
		}


		var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
		var encodedResetToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(resetToken));

		var callbackUrl = $"{passwordResetURL}/{encodedResetToken}";

		var html = "<p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Dobrý den,</p> <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">dne <strong>" + DateTime.Today.Date.ToShortDateString() + "</strong> v <strong>" + DateTime.Now.ToShortTimeString() + "</strong> jste požádal o obnovení hesla na Vašem účtu.</p> <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Prosím postupujte odkazem níže.</p> <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"btn btn-primary\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; box-sizing: border-box; width: 100%;\" width=\"100%\"> <tbody> <tr> <td align=\"left\" style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;\" valign=\"top\"> <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;\"> <tbody> <tr> <td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; border-radius: 5px; text-align: center; background-color: #3498db;\" valign=\"top\" align=\"center\" bgcolor=\"#3498db\"> <a href=\"" + HtmlEncoder.Default.Encode(callbackUrl) + "\" target=\"_blank\" style=\"border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; display: inline-block; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-decoration: none; text-transform: capitalize; background-color: #3498db; border-color: #3498db; color: #ffffff;\">Resetovat Heslo</a> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Pokud jste tuto žádost nevyvolali, email ignorujte.</p>";

		_ = _emailSender.SendEmailAsync(email, "Dagrader - Zapomenuté Heslo", html);

		return AccountOperationResult.Success;
	}


	public async Task<AccountOperationResult> SubmitPasswordReset(string encodedResetToken, string email, string password)
	{
		// TODO: Validation

		var user = await _userManager.FindByEmailAsync(email);
		var resetToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedResetToken));

		if (user is null)
		{
			// Log
			return AccountOperationResult.NotValid;
		}

		var result = await _userManager.ResetPasswordAsync(user, resetToken, password);

		if (!result.Succeeded)
		{
			// TODO: Log error
			return AccountOperationResult.Failed;
		}

		return AccountOperationResult.Success;
	}

	public Task<AccountOperationResult> SubmitPasswordReset(string originalPassword, string newPassword)
	{
		throw new NotImplementedException();
	}

	public async Task<AccountOperationResult> ConfirmEmail(string userID, string encodedToken)
	{
		var user = await _userManager.FindByIdAsync(Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(userID)));

		if (user is null)
		{
			return AccountOperationResult.NotValid;
		}

		var confirmationToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(encodedToken));
		var result = await _userManager.ConfirmEmailAsync(user, confirmationToken);

		if (!result.Succeeded)
		{
			// LOG
			return AccountOperationResult.Failed;
		}

		return AccountOperationResult.Success;
	}

	private async Task SendEmailConfirmation(AppUser newUser, string registerURL)
	{
		//TODO: Validate newUser

		var userId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(await _userManager.GetUserIdAsync(newUser)));

		var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
		var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(confirmationToken));

		var callbackUrl = $"{registerURL}/{userId}/{encodedToken}";
		var html = "<p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Dobrý den,</p> <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">pro dokončení registrace ve webové aplikaci <strong>Dagrader</strong> je potřeba potvrdit Vaší emailovou adresu odkazem níže.</p> <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Váš email bude sloužit výhradně pro účely správy účtu v aplikaci. (Zapomenuté heslo, Přihlášení)</p> <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" class=\"btn btn-primary\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; box-sizing: border-box; width: 100%;\" width=\"100%\"> <tbody> <tr> <td align=\"left\" style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; padding-bottom: 15px;\" valign=\"top\"> <table role=\"presentation\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"border-collapse: separate; mso-table-lspace: 0pt; mso-table-rspace: 0pt; width: auto;\"> <tbody> <tr> <td style=\"font-family: sans-serif; font-size: 14px; vertical-align: top; border-radius: 5px; text-align: center; background-color: #3498db;\" valign=\"top\" align=\"center\" bgcolor=\"#3498db\"> <a href=\"" + HtmlEncoder.Default.Encode(callbackUrl) + "\" target=\"_blank\" style=\"border: solid 1px #3498db; border-radius: 5px; box-sizing: border-box; cursor: pointer; display: inline-block; font-size: 14px; font-weight: bold; margin: 0; padding: 12px 25px; text-decoration: none; text-transform: capitalize; background-color: #3498db; border-color: #3498db; color: #ffffff;\">Potvrdit Email</a> </td> </tr> </tbody> </table> </td> </tr> </tbody> </table> <p style=\"font-family: sans-serif; font-size: 14px; font-weight: normal; margin: 0; margin-bottom: 15px;\">Pokud jste se nikde neregistrovali, email ignorujte.</p>";

		// TODO: Validation

		_ = _emailSender.SendEmailAsync(newUser.Email!, "Dagrader - Potvrzení Registrace", html);
	}
}