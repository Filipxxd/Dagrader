using Core.Exceptions;
using Core.Validators;
using Domain;
using Domain.Enums;
using Domain.Handlers;
using Domain.Models;
using Domain.Services;
using Domain.Tables;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Core.Services;

public sealed class CourseService(ICourseTable table, UserManager<AppUser> userManager, ISessionHandler sessionHandler, ILogger<ICourseTable> logger, EntityValidator validator) : ICourseService
{
	private readonly ICourseTable _table = table;
	private readonly UserManager<AppUser> _userManager = userManager;
	private readonly ISessionHandler _sessionManager = sessionHandler;
	private readonly EntityValidator _validator = validator;
	private readonly ILogger<ICourseTable> _logger = logger;

	public async Task<IEnumerable<Course>> GetCoursesForTeacher()
	{
		if (!await _sessionManager.IsUserInRoleAsync(Roles.Teacher))
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(GetCoursesForTeacher));
		}

		var teacherId = await _sessionManager.GetLoggedInUserId();
		return await _table.GetCoursesByTeacherId(teacherId);
	}

	public async Task<List<AppUser>> GetFilteredAvailableStudents(string searchTerm)
	{
		if (!await _sessionManager.IsUserInRoleAsync(Roles.Teacher))
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(GetFilteredAvailableStudents));
		}
		var students = await _table.GetStudentsWithoutCourse(searchTerm);
		return students.ToList();
	}

	public async Task<Course> GetCourseById(int courseId)
	{
		if (!await _sessionManager.IsUserInRoleAsync(Roles.Teacher))
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(GetCourseById));
		}

		var course = await _table.GetCourseWithStudentsById(courseId) ?? throw new NotFoundException(nameof(CourseService), courseId.ToString());

		if (course.TeacherId != await _sessionManager.GetLoggedInUserId())
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(GetCourseById));
		}

		return course;
	}

	public async Task<EntityOperationResult> AddStudentsToCourse(int courseId, IEnumerable<string> studentsIds)
	{
		if (!await _sessionManager.IsUserInRoleAsync(Roles.Teacher))
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(AddStudentsToCourse));
		}

		var course = await _table.GetCourseWithStudentsById(courseId);

		if (course is null)
		{
			return EntityOperationResult.NotFound;
		}

		if (course.TeacherId != await _sessionManager.GetLoggedInUserId())
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(GetCourseById));
		}

		foreach (var studentId in studentsIds)
		{
			var student = await _userManager.FindByIdAsync(studentId);
			if (student is null)
			{
				return EntityOperationResult.NotFound;
			}

			if (student.CourseId != null)
			{
				return EntityOperationResult.NotAllowed;
			}
		}

		return await _table.AddStudentsToCourse(courseId, studentsIds);
	}

	public async Task<EntityOperationResult> RemoveStudentFromCourse(string userId)
	{
		if (!await _sessionManager.IsUserInRoleAsync(Roles.Teacher))
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(AddStudentsToCourse));
		}

		var student = await _userManager.FindByIdAsync(userId);

		if (student is null)
		{
			return EntityOperationResult.NotFound;
		}

		if (student.CourseId is null)
		{
			return EntityOperationResult.NoActionTaken;
		}

		var course = await _table.GetCourseWithStudentsById((int)student.CourseId);

		if (course is null)
		{
			return EntityOperationResult.NotFound;
		}

		if (course.TeacherId != await _sessionManager.GetLoggedInUserId())
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(GetCourseById));
		}

		return await _table.RemoveStudentFromCourse(userId);
	}

	public async Task<EntityOperationResult> CreateCourse(Course course)
	{
		if (!await _sessionManager.IsUserInRoleAsync(Roles.Teacher))
		{
			throw new NotAuthorizedException(nameof(CourseService), nameof(CreateCourse));
		}

		course.TeacherId = await _sessionManager.GetLoggedInUserId();
		course.CreatedDate = DateTime.Now;
		course.UpdatedDate = DateTime.Now;

		var validationResult = _validator.ValidateModel<CourseValidator, Course>(course);

		if (!validationResult.IsValid)
		{
			return EntityOperationResult.EntityNotValid;
		}

		return await _table.Create(course);
	}
}
