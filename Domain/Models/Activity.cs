using Domain.Models.Base;

namespace Domain.Models;

public class Activity : ChangeTracking
{
	public int ActivityId { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public string Icon { get; set; } = string.Empty;
	public bool IsGreaterBetter { get; set; }
	public bool IsDeletable { get; set; }

	public int ActivityUnitId { get; set; }
	public ActivityUnit? ActivityUnit { get; set; }
}
