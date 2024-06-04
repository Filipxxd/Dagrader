using Domain.Models.Base;

namespace Domain.Models;

public class Criteria : ChangeTracking
{
	public int CriteriaId { get; set; }
	public string DisplayName { get; set; } = string.Empty;
	public byte ClassYear { get; set; }

	public decimal? Male_1 { get; set; }
	public decimal? Male_2 { get; set; }
	public decimal? Male_3 { get; set; }
	public decimal? Male_4 { get; set; }

	public decimal? Female_1 { get; set; }
	public decimal? Female_2 { get; set; }
	public decimal? Female_3 { get; set; }
	public decimal? Female_4 { get; set; }

	public int ActivityId { get; set; }
	public Activity? Activity { get; set; }

	public string TeacherId { get; set; } = string.Empty;
	public AppUser? Teacher { get; set; }
}
