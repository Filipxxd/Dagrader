using FluentValidation;
using FluentValidation.Results;

namespace Core.Validators;

public sealed class EntityValidator(IServiceProvider serviceProvider)
{
	private readonly IServiceProvider _serviceProvider = serviceProvider;

	public ValidationResult ValidateModel<TValidator, TModel>(TModel model) where TValidator : AbstractValidator<TModel> where TModel : class
	{
		var validator = _serviceProvider.GetService(typeof(TValidator)) as TValidator
			?? throw new NotSupportedException("Validator of this type was not found.");

		return validator.Validate(model);
	}
}
