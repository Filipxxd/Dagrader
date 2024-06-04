namespace Application.ViewModels.Base;

public abstract class PaginatedViewModelBase : ViewModelBase
{
	private string _searchTerm = string.Empty;
	public string SearchTerm
	{
		get => _searchTerm ?? string.Empty;
		set => SetValue(ref _searchTerm, value);
	}

	public virtual int[] PageSizeOptions { get; set; } = { 5, 10, 15 };
	public virtual int PageSize { get; set; } = 5;
}