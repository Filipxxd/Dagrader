using AntDesign;
using Application.ViewModels.Base;
using Domain.Enums;
using Domain.Models;
using Domain.Pagination;
using Domain.Services;

namespace Application.ViewModels.Admin;

public sealed class UserManagerViewModel : PaginatedViewModelBase
{
	private readonly IUserManagementService _userManagerService;
	private readonly ModalService _modalService;
	private readonly UserEditModal _editModal;

	public UserManagerViewModel(IUserManagementService roleManagement, ModalService modalService, UserEditModal editModal)
	{
		_userManagerService = roleManagement;
		_modalService = modalService;
		_editModal = editModal;

		_editModal.ParentRefresh += OnViewModelInitializedAsync;
	}

	public UserEditModal EditModal => _editModal;
	public PaginatedList<AppUser> UsersList { get; set; } = new();

	public override async Task OnViewModelInitializedAsync()
	{
		await SetBusyAsync(async () =>
		{
			UsersList = await _userManagerService.GetPaginatedUsersAsync(SearchTerm, UsersList.Page, PageSize);
		});
	}

	public async Task HandleSearch()
	{
		await SetBusyAsync(async () =>
		{
			UsersList = await _userManagerService.GetPaginatedUsersAsync(SearchTerm, 1, PageSize);
		});
	}

	public async Task HandleItemsPerPageChange(PaginationEventArgs args)
	{
		await SetBusyAsync(async () =>
		{
			PageSize = args.PageSize;
			UsersList = await _userManagerService.GetPaginatedUsersAsync(SearchTerm, UsersList.Page, args.PageSize);
		});
	}

	public async Task HandlePageChange(PaginationEventArgs args)
	{
		await SetBusyAsync(async () =>
		{
			PageSize = args.PageSize;
			UsersList = await _userManagerService.GetPaginatedUsersAsync(SearchTerm, args.Page, PageSize);
		});
	}

	public async Task HandleUserDelete(string userId)
	{
		await SetBusyAsync(async () =>
		{
			var options = new ConfirmOptions()
			{
				Title = "Odstranění uživatele",
				Width = 400,
				Content = "Opravdu chcete smazat uživatele? Tento krok smaže také všechny jeho uložené data.",
				OnOk = async e =>
				{
					var result = await _userManagerService.DeleteUserAsync(userId);

					if (result == EntityOperationResult.Success)
					{

					}
				}
			};

			await _modalService.CreateConfirmAsync(options);
		});
	}
}
