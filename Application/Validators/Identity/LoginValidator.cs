using Application.ViewModels;
using Core.Validators.Custom;
using FluentValidation;
using Validators;

namespace Application.Validators.Identity;

public sealed class LoginValidator : AbstractValidator<LoginViewModel>
{
	public LoginValidator()
	{
		RuleFor(r => r.Email)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.EmailAddress().WithMessage(ValidationMessages.MustBeEmail)
			.MaximumLength(64).WithMessage(ValidationMessages.InvalidLength);

		RuleFor(r => r.Password)
			.NotEmpty().WithMessage(ValidationMessages.IsRequired)
			.SetValidator(new PasswordValidator());
	}
}