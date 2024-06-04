using Application.ViewModels.Admin;
using Core.Validators.Custom;
using Domain.Enums;
using FluentValidation;
using Validators;

namespace Application.Validators;

public class UserEditModalValidator : AbstractValidator<UserEditModal>
{
	public UserEditModalValidator()
	{
		RuleFor(r => r.FirstName)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.SetValidator(new CzechAlphabetValidator())
			.MaximumLength(64).WithMessage(ValidationMessages.InvalidLength);

		RuleFor(r => r.LastName)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.SetValidator(new CzechAlphabetValidator())
			.MaximumLength(64).WithMessage(ValidationMessages.InvalidLength);

		RuleFor(r => r.Gender)
			.Must(e => e.Equals(Gender.Male) || e.Equals(Gender.Female)).WithMessage("Pohlaví musí být buď muž nebo žena!");

		RuleFor(r => r.AssignedRoleIds)
			.Must(r => r.Any()).WithMessage(ValidationMessages.IsRequiredSelection);
	}
}
