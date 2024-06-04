using Application.ViewModels;
using Core.Validators.Custom;
using FluentValidation;
using Validators;

namespace Application.Validators.Identity;

public sealed class RegisterValidator : AbstractValidator<RegisterViewModel>
{
	public RegisterValidator()
	{
		RuleFor(r => r.Email)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.EmailAddress().WithMessage(ValidationMessages.MustBeEmail)
			.MaximumLength(128).WithMessage(ValidationMessages.InvalidLength);

		RuleFor(r => r.FirstName)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.SetValidator(new CzechAlphabetValidator())
			.MaximumLength(64).WithMessage(ValidationMessages.InvalidLength);

		RuleFor(r => r.LastName)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.SetValidator(new CzechAlphabetValidator())
			.MaximumLength(64).WithMessage(ValidationMessages.InvalidLength);

		RuleFor(p => p.Password)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.SetValidator(new PasswordValidator());

		RuleFor(r => r.ConfirmPassword)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.Equal(r => r.Password).WithMessage(ValidationMessages.MustMatchPasswords);
	}
}