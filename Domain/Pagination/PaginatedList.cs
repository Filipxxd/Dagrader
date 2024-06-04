using System.Collections;

namespace Domain.Pagination;

public class PaginatedList<T> : IEnumerable<T>
{
	private readonly IEnumerable<T> _items;

	public PaginatedList()
	{
		_items = [];
	}

	public PaginatedList(IEnumerable<T> items, int totalItems, int page)
	{
		_items = items;
		TotalItems = totalItems;
		Page = page;
	}

	public int TotalItems { get; set; } = 0;
	public int Page { get; set; } = 1;

	public IEnumerator<T> GetEnumerator()
	{
		return _items.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return _items.GetEnumerator();
	}
}
