using Domain.Models.Base;

namespace Domain.Models;

public class ActivityUnit : ChangeTracking
{
	public int ActivityUnitId { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public string DisplayNameShort { get; set; } = string.Empty;
	public bool IsDeletable { get; set; } = true;
}
