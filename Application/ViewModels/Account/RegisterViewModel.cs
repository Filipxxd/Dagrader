using AntDesign;
using Application.Constants;
using Application.Maps;
using Application.Validators.Identity;
using Application.ViewModels.Base;
using Core.Validators;
using Domain.Enums;
using Domain.Handlers;
using Domain.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Components;

namespace Application.ViewModels;

public sealed class RegisterViewModel(IAccountService userService, ISessionHandler sessionService, EntityValidator viewModelValidator, NavigationManager navigationManager, MessageService messageService) : ViewModelBase
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
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public Gender Gender { get; set; } = Gender.Male;

	public override async Task OnViewModelInitializedAsync()
	{
		if (await _sessionService.IsUserAuthenticatedAsync())
		{
			_navigationManager.NavigateTo(Routes.Home);
			return;
		}
	}

	public async Task HandleRegistrationSubmit()
	{
		await SetBusyAsync(async () =>
		{
			ValidationResult = _validator.ValidateModel<RegisterValidator, RegisterViewModel>(this);

			if (!ValidationResult.IsValid)
			{
				return;
			}

			var registerURL = _navigationManager.ToAbsoluteUri(Routes.Register).ToString();

			var result = await _userService.RegisterUser(this.MapToModel(), Password, registerURL);

			if (result == AccountOperationResult.Success)
			{
				_ = _messageService.Success("Registrace úspěšná, zkontrolujte svou emailovou schránku.");
				_navigationManager.NavigateTo(Routes.Login);

			}
			else if (result == AccountOperationResult.EmailTaken)
			{
				_ = _messageService.Error("Tento email je již registrován.");
			}
			else
			{
				_ = _messageService.Error("Registrace se nepodařila. Zkuste to později.");
			}
		});
	}
}