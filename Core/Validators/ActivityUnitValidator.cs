using Domain.Models;
using FluentValidation;

namespace Core.Validators;

public class ActivityUnitValidator : AbstractValidator<ActivityUnit>
{
    public ActivityUnitValidator()
    {
        RuleFor(r => r.DisplayName)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(r => r.DisplayNameShort)
            .NotEmpty()
            .MaximumLength(32);
    }
}
