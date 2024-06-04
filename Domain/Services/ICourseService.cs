using Domain.Enums;
using Domain.Models;

namespace Domain.Services;

public interface ICourseService
{
	Task<IEnumerable<Course>> GetCoursesForTeacher();
	Task<List<AppUser>> GetFilteredAvailableStudents(string searchTerm);
	Task<EntityOperationResult> CreateCourse(Course course);
	Task<EntityOperationResult> AddStudentsToCourse(int courseId, IEnumerable<string> studentsIds);
	Task<EntityOperationResult> RemoveStudentFromCourse(string userId);
	Task<Course> GetCourseById(int courseId);
}
