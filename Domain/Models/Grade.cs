using Domain.Models.Base;

namespace Domain.Models;

public class Grade : ChangeTracking
{
	public int GradeId { get; set; }
	public byte Value { get; set; }

	public int PerformanceId { get; set; }
	public Performance? Performance { get; set; }
}
