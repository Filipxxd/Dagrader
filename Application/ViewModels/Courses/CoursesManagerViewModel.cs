using Application.ViewModels.Base;
using Domain.Models;
using Domain.Services;

namespace Application.ViewModels.Courses;

public sealed class CoursesManagerViewModel : ViewModelBase
{
	private readonly ICourseService _courseService;
	private readonly CourseCreateModal _createModal;

	public CoursesManagerViewModel(ICourseService courseService, CourseCreateModal courseCreateModal)
	{
		_createModal = courseCreateModal;
		_courseService = courseService;

		_createModal.ParentRefresh += OnViewModelInitializedAsync;
	}

	public CourseCreateModal CreateModal => _createModal;

	private IEnumerable<Course> _courses = [];
	public IEnumerable<Course> Courses
	{
		get => _courses;
		private set => SetValue(ref _courses, value);
	}

	public override async Task OnViewModelInitializedAsync()
	{
		Courses = await _courseService.GetCoursesForTeacher();
	}
}

