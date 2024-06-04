using Application.ViewModels;
using Application.ViewModels.Admin;
using Domain.Models;

namespace Application.Maps;

internal static class UserMapper
{
	public static void MapTo(this AppUser domainModel, UserEditModal viewModel)
	{
		viewModel.UserId = domainModel.Id;
		viewModel.Email = domainModel.Email ?? string.Empty;
		viewModel.FirstName = domainModel.FirstName;
		viewModel.LastName = domainModel.LastName;
		viewModel.Gender = domainModel.Gender;
		viewModel.IsLocked = domainModel.LockoutEnd > DateTime.UtcNow;
		viewModel.AssignedRoleIds = domainModel.Roles.Select(role => role.Id).ToList();
	}

	public static AppUser MapToModel(this RegisterViewModel viewModel)
	{
		return new()
		{
			Email = viewModel.Email,
			FirstName = viewModel.FirstName,
			LastName = viewModel.LastName,
			Gender = viewModel.Gender
		};
	}

	public static AppUser MapToModel(this UserEditModal viewModel)
	{
		return new()
		{
			Id = viewModel.UserId,
			FirstName = viewModel.FirstName,
			LastName = viewModel.LastName,
			Gender = viewModel.Gender
		};
	}
}
