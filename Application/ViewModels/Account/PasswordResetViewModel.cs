using AntDesign;
using Application.Constants;
using Application.Validators;
using Application.Validators.Identity;
using Application.ViewModels.Base;
using Core.Validators;
using Domain.Enums;
using Domain.Handlers;
using Domain.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace Application.ViewModels.Identity;

public sealed class PasswordResetViewModel(IAccountService userService, ISessionHandler sessionService, EntityValidator viewModelValidator, NavigationManager navigationManager, MessageService messageService) : ViewModelBase
{
	private readonly IAccountService _userService = userService;
	private readonly ISessionHandler _sessionService = sessionService;
	private readonly EntityValidator _validator = viewModelValidator;
	private readonly NavigationManager _navigationManager = navigationManager;
	private readonly MessageService _messageService = messageService;

	private ValidationResult _validationResult = new();
	public ValidationResult ValidationResult
	{
		get => _validationResult;
		set => SetValue(ref _validationResult, value);
	}

	public string Email { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
	public string ConfirmPassword { get; set; } = string.Empty;

	public override async Task OnViewModelInitializedAsync()
	{
		if (await _sessionService.IsUserAuthenticatedAsync())
		{
			_navigationManager.NavigateTo(Routes.Home);
			return;
		}
	}

	public async Task SubmitRequestForReset()
	{
		await SetBusyAsync(async () =>
		{
			ValidationResult = _validator.ValidateModel<ForgotPasswordValidator, PasswordResetViewModel>(this);

			var isEmailValid = string.IsNullOrEmpty(ValidationResult.GetErrorMessageFor(nameof(Email)));

			if (!isEmailValid)
			{
				return;
			}

			var passwordResetURL = _navigationManager.ToAbsoluteUri(Routes.PasswordReset).ToString();

			await _userService.SendEmailResetPassword(Email, passwordResetURL);

			_ = _messageService.Success("Zkontrolujte svou emailovou schránku.");

			_navigationManager.NavigateTo(Routes.Login);
		});
	}

	public async Task SubmitPasswordReset(string code)
	{
		await SetBusyAsync(async () =>
		{
			ValidationResult = _validator.ValidateModel<ForgotPasswordValidator, PasswordResetViewModel>(this);

			if (!ValidationResult.IsValid)
			{
				return;
			}

			var result = await _userService.SubmitPasswordReset(code, Email, Password);

			if (result == AccountOperationResult.Success)
			{
				_ = _messageService.Success("Heslo úspěšně změněno, můžete se přihlásit.");
			}
			else
			{
				_ = _messageService.Success("Nastala chyba, zkuste to později.");
			}

			_navigationManager.NavigateTo(Routes.Login);
		});
	}
}
