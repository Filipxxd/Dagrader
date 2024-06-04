namespace Domain.Enums;

public enum EntityOperationResult
{
	Success,
	InternalServerError,
	NotFound,
	EntityNotValid,
	NoActionTaken,
	NotAllowed,
	NotAuthorized,
	Warning
}
