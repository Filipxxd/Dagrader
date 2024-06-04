namespace Application.ViewModels.Base;

public class ModalBaseViewModel : ViewModelBase
{
	private bool isVisible = false;
	public bool IsVisible
	{
		get => isVisible;
		set => SetValue(ref isVisible, value);
	}

	public virtual async Task HandleShowModal()
	{
		await SetBusyAsync(() =>
		{
			IsVisible = true;
			return Task.CompletedTask;
		});
	}

	public virtual async Task HandleShowModal(string id)
	{
		await SetBusyAsync(() =>
		{
			IsVisible = true;
			return Task.CompletedTask;
		});
	}

	public virtual async Task HandleConfirm()
	{
		await SetBusyAsync(() =>
		{
			IsVisible = false;
			return Task.CompletedTask;
		});
	}

	public virtual async Task HandleCancel()
	{
		await SetBusyAsync(() =>
		{
			IsVisible = false;
			return Task.CompletedTask;
		});
	}
}
