using Domain.Models.Base;

namespace Domain.Models;

public class Performance : ChangeTracking
{
	public int PerformanceId { get; set; }
	public decimal Value { get; set; }
	public DateTime? ApprovedDate { get; set; }
	public bool IsApproved { get; set; } = false;
	public string? FeedbackMessage { get; set; }

	public string StudentId { get; set; } = string.Empty;
	public AppUser? Student { get; set; }

	public int RequiredPerformanceId { get; set; }
	public RequiredPerformance? RequiredPerformance { get; set; }
}
