using Application.ViewModels.Base;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace Web.Pages;

public class PageComponentBase<TModel> : ComponentBase, IDisposable
	where TModel : ViewModelBase
{
	[Inject, Parameter]
	public TModel Model { get; set; } = default!;

	[Inject]
	public NavigationManager _navigationManager { get; set; } = default!;

	protected override bool ShouldRender()
	{
		return !Model.IsBusy;
	}

	protected override async Task OnInitializedAsync()
	{
		Model.PropertyChanged += OnModelPropertyChanged;
		await Model.OnViewModelInitializedAsync();
		await base.OnInitializedAsync();
	}

	protected override async Task OnParametersSetAsync()
	{
		await Model.OnViewModelParametersSetAsync();
		await base.OnParametersSetAsync();
	}

	private async void OnModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
	{
		await InvokeAsync(StateHasChanged);
	}

	public virtual void Dispose()
	{
		Model.PropertyChanged -= OnModelPropertyChanged;
	}
}
