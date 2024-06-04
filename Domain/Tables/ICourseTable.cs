using Domain.Enums;
using Domain.Models;

namespace Domain.Tables;

public interface ICourseTable
{
	Task<IEnumerable<Course>> GetCoursesByTeacherId(string id);
	Task<IEnumerable<AppUser>> GetStudentsWithoutCourse(string searchTerm);
	Task<Course?> GetCourseWithStudentsById(int id);
	Task<EntityOperationResult> AddStudentsToCourse(int courseId, IEnumerable<string> studentIds);
	Task<EntityOperationResult> RemoveStudentFromCourse(string userId);
	Task<EntityOperationResult> Create(Course entity);
	Task<EntityOperationResult> Update(Course entity);
	Task<EntityOperationResult> Delete(int id);
}

