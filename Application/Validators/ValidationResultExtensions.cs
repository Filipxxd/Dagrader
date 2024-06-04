using FluentValidation.Results;

namespace Application.Validators;

public static class ValidationResultExtensions
{
	public static string? GetErrorMessageFor(this ValidationResult validationResult, string propertyName)
	{
		return validationResult.Errors.Find(e => e.PropertyName == propertyName)?.ErrorMessage;
	}
}
