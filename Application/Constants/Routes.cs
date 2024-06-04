namespace Application.Constants;

public static class Routes
{
	public const string Home = "/";

	// Identity
	public const string IdentityDefault = "/Identity";
	public const string Profile = "/profil";
	public const string Login = "/prihlasit-se";
	public const string Logout = "/odhlasit-se";
	public const string Register = "/registrovat-se";
	public const string PasswordReset = "/zapomenute-heslo";

	// Non-Authorized
	public const string HallOfFame = "/sin-slavy";
	public const string About = "/o-aplikaci";

	// Admin
	public const string UserManager = "/spravce-uzivatelu";

	// Teacher
	public const string Activities = "/sportovni-aktivity";
	public const string Criterias = "/kriteria-hodnoceni";
	public const string Courses = "/tridy";
	public const string CourseDetail = "/detail-tridy/";

	// Student
	public const string Performances = "/vykony";
}