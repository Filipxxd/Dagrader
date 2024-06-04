using Application.ViewModels.Courses;
using Domain.Models;

namespace Application.Maps;

internal static class CourseMapper
{
	public static Course MapToModel(this CourseCreateModal viewModel)
	{
		return new()
		{
			ClassYear = viewModel.ClassYear,
			ClassSymbol = viewModel.ClassSymbol,
			AcademicYear = viewModel.AcademicYear
		};
	}

	public static void MapTo(this Course model, CourseDetailViewModel viewModel)
	{
		viewModel.CourseId = model.CourseId;
		viewModel.AcademicYear = model.AcademicYear;
		viewModel.ClassYear = model.ClassYear;
		viewModel.ClassSymbol = model.ClassSymbol;
		viewModel.AcademicYear = model.AcademicYear;

		viewModel.AssignedStudents = model.Students.Select(s => new KeyValuePair<string, string>(s.Id, s.FullName)).ToList();
	}
}
