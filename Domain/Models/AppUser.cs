using Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models;

public class AppUser : IdentityUser
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public Gender Gender { get; set; }

	public int? CourseId { get; set; }
	public Course? Course { get; set; }

	public List<IdentityRole> Roles { get; set; } = [];

	public string FullName => $"{FirstName} {LastName}";
}
