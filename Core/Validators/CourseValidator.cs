using Domain.Models;
using FluentValidation;

namespace Core.Validators;

public sealed class CourseValidator : AbstractValidator<Course>
{
	public CourseValidator()
	{
		RuleFor(c => c.AcademicYear)
			.Length(9)
			.Matches("\\d{4}\\S\\d{4}");

		RuleFor(c => c.ClassSymbol)
			.NotEmpty();

		RuleFor(c => c.ClassYear)
			.Must(b => b > 0 && b < 6);
	}
}
