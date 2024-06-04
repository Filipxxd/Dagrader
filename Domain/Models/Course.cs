using Domain.Models.Base;

namespace Domain.Models;

public class Course : ChangeTracking
{
	public int CourseId { get; set; }
	public string AcademicYear { get; set; } = string.Empty; // e.g. "2023/2024"
	public byte ClassYear { get; set; }
	public char ClassSymbol { get; set; }

	public string TeacherId { get; set; } = string.Empty;
	public AppUser? Teacher { get; set; }

	public List<AppUser> Students { get; set; } = [];
}
