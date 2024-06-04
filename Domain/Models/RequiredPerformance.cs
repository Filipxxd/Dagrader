using Domain.Models.Base;

namespace Domain.Models;

public class RequiredPerformance : ChangeTracking
{
	public int RequiredPerformanceId { get; set; }
	public DateTime EntryStartDate { get; set; }
	public string? FeedbackMessage { get; set; }

	public int? CourseId { get; set; }
	public Course? Course { get; set; }

	public int ActivityId { get; set; }
	public Activity? Activity { get; set; }
}
