namespace Domain.Enums;

public enum AccountOperationResult
{
	Success,
	EmailTaken,
	EmailNotConfirmed,
	NotValid,
	LockedOut,
	Failed
}
