using Dapper;
using Domain;
using Domain.Enums;
using Domain.Models;
using Domain.Tables;
using Microsoft.Extensions.Logging;
using static Dapper.SqlMapper;

namespace Data.Tables;

public sealed class CourseTable(IDbConnectionFactory dbConnectionFactory, ILogger<CourseTable> logger) : DbConnectionTable(dbConnectionFactory), ICourseTable
{
	private readonly ILogger<CourseTable> _logger = logger;

	public async Task<IEnumerable<Course>> GetCoursesByTeacherId(string teacherId)
	{
		const string sql = @"
			SELECT c.* FROM Courses c
			LEFT JOIN AspNetUsers u ON u.Id = c.TeacherId
			WHERE c.TeacherId = @TeacherId;";

		return await DbConnection.QueryAsync<Course>(sql, new { TeacherId = teacherId });
	}

	public async Task<IEnumerable<AppUser>> GetStudentsWithoutCourse(string searchTerm)
	{
		const string sql = @"
			SELECT TOP 10 u.* 
			FROM AspNetUsers u
			LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
			LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
			WHERE (r.Name = @RoleStudent 
				AND u.EmailConfirmed = 1
				AND u.CourseId IS NULL 
				AND (u.FirstName LIKE @SearchTerm OR u.LastName LIKE @SearchTerm OR CONCAT(u.FirstName, ' ', u.LastName) LIKE @SearchTerm))
			ORDER BY u.LastName DESC;";

		var parameters = new { RoleStudent = Roles.Student, SearchTerm = $"%{searchTerm}%" };
		return await DbConnection.QueryAsync<AppUser>(sql, parameters);
	}

	public async Task<Course?> GetCourseWithStudentsById(int courseId)
	{
		const string sql = @"
			SELECT c.*, u.Id, u.FirstName, u.LastName, u.CourseId 
			FROM Courses c
			LEFT JOIN AspNetUsers u ON u.CourseId = c.CourseId
			WHERE c.CourseId = @CourseId;";

		var courseDictionary = new Dictionary<int, Course>();

		var result = await DbConnection.QueryAsync<Course, AppUser, Course>(
			sql,
			(course, user) =>
			{
				if (!courseDictionary.TryGetValue(course.CourseId, out var currentCourse))
				{
					currentCourse = course;
					courseDictionary.Add(currentCourse.CourseId, currentCourse);
				}

				if (user != null)
				{
					currentCourse.Students.Add(user);
				}

				return currentCourse;
			},
			new { CourseId = courseId },
			splitOn: "Id"
		);

		return result.FirstOrDefault();
	}

	public async Task<EntityOperationResult> Create(Course entity)
	{
		using var transaction = DbConnection.BeginTransaction();

		const string sql = @"
			INSERT INTO Courses (AcademicYear, ClassYear, ClassSymbol, TeacherId, CreatedDate, UpdatedDate) 
			VALUES (@AcademicYear, @ClassYear, @ClassSymbol, @TeacherId, @CreatedDate, @UpdatedDate);";

		try
		{
			var rowsAffected = await DbConnection.ExecuteAsync(sql, entity, transaction);
			transaction.Commit();

			return rowsAffected == 1 ? EntityOperationResult.Success : EntityOperationResult.NoActionTaken;

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while creating the course.");
			transaction.Rollback();
		}

		return EntityOperationResult.InternalServerError;
	}

	public async Task<EntityOperationResult> AddStudentsToCourse(int courseId, IEnumerable<string> studentIds)
	{
		using var transaction = DbConnection.BeginTransaction();

		const string sql = @"UPDATE AspNetUsers SET CourseId = @CourseId WHERE Id = @UserId;";

		try
		{
			foreach (var studentId in studentIds)
			{
				var parameters = new { CourseId = courseId, UserId = studentId };

				await DbConnection.ExecuteAsync(sql, parameters, transaction);
			}

			transaction.Commit();

			return EntityOperationResult.Success;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while adding student to course: '{courseId}'.", courseId);
			transaction.Rollback();
		}

		return EntityOperationResult.InternalServerError;
	}

	// TODO: Remove grades!
	public async Task<EntityOperationResult> RemoveStudentFromCourse(string userId)
	{
		using var transaction = DbConnection.BeginTransaction();

		const string sql = @"UPDATE AspNetUsers SET CourseId = NULL WHERE Id = @UserId;";

		try
		{
			var rowsAffected = await DbConnection.ExecuteAsync(sql, new { UserId = userId }, transaction);
			transaction.Commit();

			return rowsAffected == 1 ? EntityOperationResult.Success : EntityOperationResult.NoActionTaken;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while removing student from course: '{userId}''.", userId);
			transaction.Rollback();
		}

		return EntityOperationResult.InternalServerError;
	}

	public async Task<EntityOperationResult> Update(Course entity)
	{
		using var transaction = DbConnection.BeginTransaction();

		const string sql = @"
			UPDATE Courses 
			SET AcademicYear = @AcademicYear, ClassYear = @ClassYear, ClassSymbol = @ClassSymbol, UpdatedDate = @DateUpdated 
			WHERE CourseId = @CourseId;";

		try
		{
			var rowsAffected = await DbConnection.ExecuteAsync(sql, entity, transaction);
			transaction.Commit();

			return rowsAffected == 1 ? EntityOperationResult.Success : EntityOperationResult.NoActionTaken;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while updating the course with id: '{courseId}'.", entity.CourseId);
			transaction.Rollback();
		}

		return EntityOperationResult.InternalServerError;

	}

	public async Task<EntityOperationResult> Delete(int id)
	{
		throw new NotImplementedException();
	}
}
