namespace Domain.Enums;

public enum AccountOperationResult
{
	PasswordInvalid,
	EmailTaken,
	EmailNotConfirmed,
	EmailInvalid,
	CredentialsInvalid,
	Success,
	LockedOut,
	Failed
}
