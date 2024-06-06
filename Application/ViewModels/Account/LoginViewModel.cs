using AntDesign;
using Application.Auth;
using Application.Constants;
using Application.Validators.Identity;
using Application.ViewModels.Base;
using Core.Validators;
using Domain.Enums;
using Domain.Handlers;
using Domain.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace Application.ViewModels;

public sealed class LoginViewModel(IAccountService userService, ISessionHandler sessionHandler, EntityValidator validator, NavigationManager navigationManager, MessageService messageService)
	: ViewModelBase
{
	private readonly IAccountService _userService = userService;
	private readonly ISessionHandler _sessionHandler = sessionHandler;
	private readonly EntityValidator _validator = validator;
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
	public bool IsPersistant { get; set; } = false;

	public override async Task OnViewModelInitializedAsync()
	{
		if (await _sessionHandler.IsUserAuthenticatedAsync())
		{
			_navigationManager.NavigateTo(Routes.Home);
			return;
		}
	}

	public async Task HandleLoginSubmit()
	{
		await SetBusyAsync(async () =>
		{
			ValidationResult = _validator.ValidateModel<LoginValidator, LoginViewModel>(this);

			if (!ValidationResult.IsValid)
			{
				return;
			}

			var result = await _userService.CheckLoginForUser(Email, Password);

			switch (result)
			{
				case AccountOperationResult.Success:
					{
						var key = AuthenticationMiddleware.Add(new(Email, Password, IsPersistant));

						// forceLoad - re-trigger AuthenticationMiddleware
						_navigationManager.NavigateTo($"{Routes.Login}?{AuthenticationMiddleware.Key}={key}", true);
						break;
					}

				case AccountOperationResult.EmailNotConfirmed:
					{
						_ = _messageService.Error("Váš email nebyl dosud potvrzen.");
						break;
					}
				case AccountOperationResult.LockedOut:
					{
						_ = _messageService.Error("Váš účet byl dočasně zablokován.");
						break;
					}

				default:
					{
						_ = _messageService.Error("Neplatný email nebo heslo.");
						break;
					}
			}

		});
	}
}