using FluentValidation;

namespace Core.Validators.Custom;

public sealed class PasswordValidator : AbstractValidator<string>
{
	public PasswordValidator()
	{
		RuleFor(password => password)
			.MinimumLength(6).WithMessage("Délka musí být alespoň 6 znaků.")
			.MaximumLength(16).WithMessage("Délka nesmí překročit 16 znaků.")
			.Matches(@"[A-Z]+").WithMessage("Musí obsahovat alespoň 1 velké písmeno.")
			.Matches(@"[a-z]+").WithMessage("Musí obsahovat alespoň 1 malé písmeno.")
			.Matches(@"[0-9]+").WithMessage("Musí obsahovat alespoň 1 číslo.")
			.Matches(@"[\!\?\*\._-]+").WithMessage("Musí obsahovat alespoň 1 z těchto speciálních znaků: '! ? * . _ -'");
	}
}
