using Application.ViewModels.Base;
using Domain.Handlers;

namespace Application.ViewModels;

public sealed class IndexViewModel(ISessionHandler sessionHandler) : ViewModelBase
{
	private readonly ISessionHandler _sessionHandler = sessionHandler;

	public string LoginStatusMessage { get; set; } = string.Empty;

	public override async Task OnViewModelParametersSetAsync()
	{
		LoginStatusMessage = await _sessionHandler.IsUserAuthenticatedAsync() ? "Přihlášen" : "Nepřihlášen";
	}
}
