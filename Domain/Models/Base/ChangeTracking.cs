namespace Domain.Models.Base;

public abstract class ChangeTracking
{
	public DateTime CreatedDate { get; set; }
	public DateTime UpdatedDate { get; set; }
}
