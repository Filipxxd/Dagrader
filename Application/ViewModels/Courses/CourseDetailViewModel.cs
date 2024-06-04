using AntDesign;
using Application.Maps;
using Application.ViewModels.Base;
using Domain.Enums;
using Domain.Services;

namespace Application.ViewModels.Courses;

public sealed class CourseDetailViewModel : ViewModelBase
{
	private readonly ICourseService _courseService;
	private readonly ModalService _modalService;
	private readonly MessageService _messageService;
	private readonly StudentAddModal _studentAddModal;

	public CourseDetailViewModel(ICourseService courseService, StudentAddModal studentAddModal, ModalService modalService, MessageService messageService)
	{
		_courseService = courseService;
		_studentAddModal = studentAddModal;
		_modalService = modalService;
		_messageService = messageService;

		_studentAddModal.ParentRefresh += OnViewModelParametersSetAsync;
	}

	public StudentAddModal StudentAddModal => _studentAddModal;

	private List<KeyValuePair<string, string>> _assignedStudents = [];
	public List<KeyValuePair<string, string>> AssignedStudents
	{
		get => _assignedStudents;
		set => SetValue(ref _assignedStudents, value);
	}
	public List<string> GradesToApprove { get; set; } = [];

	public int CourseId { get; set; }
	public string AcademicYear { get; set; } = string.Empty;
	public byte ClassYear { get; set; }
	public char ClassSymbol { get; set; }
	public string Class => $"{ClassYear}.{ClassSymbol}";

	public override async Task OnViewModelParametersSetAsync()
	{
		(await _courseService.GetCourseById(CourseId))?.MapTo(this);
		StudentAddModal.CourseId = CourseId;
	}

	public async Task HandleStudentRemove(string userId)
	{
		await SetBusyAsync(async () =>
		{
			var options = new ConfirmOptions()
			{
				Title = "Vyhození studenta",
				Width = 400,
				Content = "Opravdu chcete vyhodit studenta ze třídy?",
				OnOk = async e =>
				{
					var result = await _courseService.RemoveStudentFromCourse(userId);

					if (result == EntityOperationResult.Success)
					{
						(await _courseService.GetCourseById(CourseId))?.MapTo(this);

						_ = _messageService.Success("Student úspěšně vyhozen.");
					}
					else
					{
						_ = _messageService.Error("Něco se pokazilo. Zkuste to později.");
					}
				}
			};

			await _modalService.CreateConfirmAsync(options);
		});
	}
}
