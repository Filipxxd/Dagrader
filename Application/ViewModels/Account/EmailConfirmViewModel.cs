using AntDesign;
using Application.Constants;
using Application.ViewModels.Base;
using Domain.Enums;
using Domain.Handlers;
using Domain.Services;
using Microsoft.AspNetCore.Components;

namespace Application.ViewModels.Identity;

public sealed class EmailConfirmViewModel(IAccountService userService, ISessionHandler sessionHandler, NavigationManager navigationManager, MessageService messageService) : ViewModelBase
{
	private readonly IAccountService _userService = userService;
	private readonly ISessionHandler _sessionHandler = sessionHandler;
	private readonly NavigationManager _navigationManager = navigationManager;
	private readonly MessageService _messageService = messageService;

	public override async Task OnViewModelInitializedAsync()
	{
		if (await _sessionHandler.IsUserAuthenticatedAsync())
		{
			_navigationManager.NavigateTo(Routes.Home);
			return;
		}
	}

	public async Task HandleEmailConfirmation(string userId, string token)
	{
		await SetBusyAsync(async () =>
		{
			var result = await _userService.ConfirmEmail(userId, token);

			if (result != EntityOperationResult.Success)
			{
				_ = _messageService.Error("Při ověřování emailu nastala chyba.");
			}
			else
			{
				_ = _messageService.Success("Email úspěšně ověřen, můžete se přihlásit.");
			}

			_navigationManager.NavigateTo(Routes.Login);
		});
	}
}
