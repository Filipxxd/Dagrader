using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Application.ViewModels.Base;

public abstract class ViewModelBase : INotifyPropertyChanged
{
	private bool _isBusy;
	public bool IsBusy
	{
		get => _isBusy;
		set => SetValue(ref _isBusy, value);
	}

	public delegate Task ViewModelInitializedEventHandler();
	public delegate Task ViewModelParametersSetEventHandler();

	public event ViewModelInitializedEventHandler? ViewModelInitialized;
	public event ViewModelParametersSetEventHandler? ViewModelParametersSet;
	public event PropertyChangedEventHandler? PropertyChanged;

	public virtual async Task OnViewModelInitializedAsync()
	{
		if (ViewModelInitialized != null) await ViewModelInitialized.Invoke();
	}

	public virtual async Task OnViewModelParametersSetAsync()
	{
		if (ViewModelParametersSet != null) await ViewModelParametersSet.Invoke();
	}

	protected void OnPropertyChanged([CallerMemberName] string propertyName = default!)
	{
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected void SetValue<T>(ref T backingFiled, T value, [CallerMemberName] string propertyName = default!)
	{
		if (EqualityComparer<T>.Default.Equals(backingFiled, value)) return;
		backingFiled = value;
		OnPropertyChanged(propertyName);
	}

	protected async Task SetBusyAsync(Func<Task> action)
	{
		if (IsBusy) return;
		IsBusy = true;

		try
		{
			await action();
		}
		finally
		{
			IsBusy = false;
		}
	}
}