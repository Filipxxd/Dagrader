using Application.ViewModels.Courses;
using FluentValidation;
using Validators;

namespace Application.Validators.Courses;

public sealed class CourseCreateValidator : AbstractValidator<CourseCreateModal>
{
	public CourseCreateValidator()
	{
		RuleFor(c => c.ClassSymbol)
			.Must((c, s) => c.AvailableSymbols.Contains(s)).WithMessage((c, s) => $"Musí být z výběru: {c.AvailableSymbols}");

		RuleFor(c => c.ClassYear)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.InclusiveBetween((byte)1, (byte)4).WithMessage("Musí být v rozmezí 1 - 4.");

		RuleFor(c => c.AcademicYear)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.Must(year =>
			{
				if (year.Length != 9 || year[4] != '/') return false;

				var parts = year.Split('/');
				if (parts.Length != 2) return false;

				if (!int.TryParse(parts[0], out int startYear) || !int.TryParse(parts[1], out int endYear)) return false;

				if (endYear <= startYear) return false;

				return true;
			}).WithMessage("Nesprávný formát, musí být např. '2010/2011'.");
	}
}

public static class AcademicYearHelper
{
	public static string GetCurrentAcademicYear()
	{
		var currentDate = DateTime.Now;

		int startYear;
		int endYear;

		if (currentDate.Month >= 9)
		{
			startYear = currentDate.Year;
			endYear = startYear + 1;
		}
		else
		{
			endYear = currentDate.Year;
			startYear = endYear - 1;
		}

		return $"{startYear}/{endYear}";
	}
}
