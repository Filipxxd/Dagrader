using FluentValidation;

namespace Core.Validators.Custom;

public sealed class CzechAlphabetValidator : AbstractValidator<string>
{
	public CzechAlphabetValidator()
	{
		RuleFor(text => text)
			.Matches(@"^[a-zA-ZáčďéěíňóřšťúůýžÁČĎÉĚÍŇÓŘŠŤÚŮÝŽ\s]+$").WithMessage("Musí obsahovat pouze znaky české abecedy.");
	}
}
