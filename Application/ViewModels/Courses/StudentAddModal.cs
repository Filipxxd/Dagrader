using AntDesign;
using Application.ViewModels.Base;
using Domain.Enums;
using Domain.Models;
using Domain.Services;

namespace Application.ViewModels.Courses;

public sealed class StudentAddModal(ICourseService courseService, MessageService messageService) : ModalBaseViewModel
{
	private readonly ICourseService _courseService = courseService;
	private readonly MessageService _messageService = messageService;

	public event Func<Task>? ParentRefresh;

	public int CourseId { get; set; }
	public string SearchTerm { get; set; } = string.Empty;

	private List<AppUser> _availableStudents = [];
	public List<AppUser> AvailableStudents
	{
		get => _availableStudents;
		private set => SetValue(ref _availableStudents, value);
	}

	private List<AppUser> _selectedStudents = [];
	public List<AppUser> SelectedStudents
	{
		get => _selectedStudents;
		private set => SetValue(ref _selectedStudents, value);
	}

	public override async Task HandleShowModal()
	{
		await SetBusyAsync(async () =>
		{
			SelectedStudents.Clear();
			AvailableStudents = await _courseService.GetFilteredAvailableStudents(SearchTerm);
			IsVisible = true;
		});
	}

	public async Task HandleSearch()
	{
		await SetBusyAsync(async () =>
		{
			AvailableStudents = await _courseService.GetFilteredAvailableStudents(SearchTerm);
			AvailableStudents.RemoveAll(u => SelectedStudents.Any(su => su.Id == u.Id));
		});
	}

	public void HandleSelect(AppUser user)
	{
		SelectedStudents.Add(user);
		AvailableStudents.Remove(user);
	}

	public async Task HandleDeselect(AppUser user)
	{
		await SetBusyAsync(async () =>
		{
			SelectedStudents.Remove(user);
			AvailableStudents = await _courseService.GetFilteredAvailableStudents(SearchTerm);
			AvailableStudents.RemoveAll(au => SelectedStudents.Any(su => su.Id == au.Id));
		});
	}

	public override async Task HandleConfirm()
	{
		await SetBusyAsync(async () =>
		{
			if (SelectedStudents.Count == 0)
			{
				return;
			}

			var result = await _courseService.AddStudentsToCourse(CourseId, SelectedStudents.Select(s => s.Id));

			if (result == EntityOperationResult.Success)
			{
				if (ParentRefresh != null) await ParentRefresh.Invoke();

				_ = _messageService.Success("Student(i) úspěšně přidán(i).");
			}
			else
			{
				_ = _messageService.Error("Něco se pokazilo. Zkuste to později.");
			}

			IsVisible = false;
		});
	}
}