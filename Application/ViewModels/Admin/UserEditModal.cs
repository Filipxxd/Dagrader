using AntDesign;
using Application.Maps;
using Application.Validators;
using Application.ViewModels.Base;
using Core.Validators;
using Domain.Enums;
using Domain.Models;
using Domain.Services;
using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace Application.ViewModels.Admin;

public sealed class UserEditModal(UserManager<AppUser> userManager, IUserManagementService userManagementService, EntityValidator validationService, MessageService messageService) : ModalBaseViewModel
{
	private readonly UserManager<AppUser> _userManager = userManager;
	private readonly IUserManagementService _userManagementService = userManagementService;
	private readonly EntityValidator _validationService = validationService;
	private readonly MessageService _messageService = messageService;

	private ValidationResult validationResult = new();
	public ValidationResult ValidationResult
	{
		get => validationResult;
		set => SetValue(ref validationResult, value);
	}

	public event Func<Task>? ParentRefresh;

	public IEnumerable<KeyValuePair<string, string>> AvailableRoles { get; set; } = [];
	public IEnumerable<string> AssignedRoleIds { get; set; } = [];

	public string UserId { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public Gender Gender { get; set; }
	public bool IsLocked { get; set; }

	public override async Task HandleShowModal(string userId)
	{
		await SetBusyAsync(async () =>
		{
			AssignedRoleIds = [];
			AvailableRoles = (await _userManagementService.GetAllRolesAsync()).Select(role => new KeyValuePair<string, string>(role.Id, role.Name));
			ValidationResult = new();

			var user = await _userManagementService.GetUserWithRolesAsync(userId);

			if (user != null)
			{
				user.MapTo(this);
				IsVisible = true;
			}
			else
			{
				_ = _messageService.Error("Nelze editovat.");
			}
		});
	}

	public override async Task HandleConfirm()
	{
		await SetBusyAsync(async () =>
		{
			ValidationResult = _validationService.ValidateModel<UserEditModalValidator, UserEditModal>(this);

			if (ValidationResult.IsValid)
			{
				var result = await _userManagementService.UpdateUser(this.MapToModel(), AssignedRoleIds);

				if (result == EntityOperationResult.Success)
				{
					if (ParentRefresh != null) await ParentRefresh.Invoke();
					IsVisible = false;

					_ = _messageService.Success("Změny uloženy.");
				}
				else
				{
					_ = _messageService.Error("Něco se pokazilo. Zkuste to později.");
				}
			}
		});
	}
}
