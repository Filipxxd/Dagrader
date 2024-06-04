using AntDesign;
using Application.Maps;
using Application.Validators.Courses;
using Application.ViewModels.Base;
using Core.Validators;
using Domain.Enums;
using Domain.Services;
using FluentValidation.Results;

namespace Application.ViewModels.Courses;

public sealed class CourseCreateModal(ICourseService courseService, MessageService messageService, EntityValidator validator) : ModalBaseViewModel
{
	private readonly ICourseService _courseService = courseService;
	private readonly MessageService _messageService = messageService;
	private readonly EntityValidator _validator = validator;

	public event Func<Task>? ParentRefresh;

	private ValidationResult validationResult = new();
	public ValidationResult ValidationResult
	{
		get => validationResult;
		set => SetValue(ref validationResult, value);
	}

	public byte ClassYear { get; set; }
	public char ClassSymbol { get; set; }
	public string AcademicYear { get; set; } = "2023/2024";

	public char[] AvailableSymbols { get; set; } = ['A', 'B', 'C', 'D', 'E'];

	public override async Task HandleConfirm()
	{
		await SetBusyAsync(async () =>
		{
			ValidationResult = _validator.ValidateModel<CourseCreateValidator, CourseCreateModal>(this);

			if (!ValidationResult.IsValid)
			{
				return;
			}

			var result = await _courseService.CreateCourse(this.MapToModel());

			if (result == EntityOperationResult.Success)
			{
				if (ParentRefresh != null) await ParentRefresh.Invoke();
				IsVisible = false;

				_ = _messageService.Success("Třída úspěšně vytvořena.");
			}
			else
			{
				_ = _messageService.Error("Něco se pokazilo. Zkuste to později.");
			}
		});
	}
}
