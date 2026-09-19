using MyProject.Models;
using MyProject.Models.DTOs;

namespace MyProject.Repositories.IRepositories
{
    public interface IStudentRepository : IGenericRepository<Student>
    {
        Task<IEnumerable<StudentDTO>> GetAllWithAbsencesAsync();
        Task<StudentDTO?> GetStudentWithAbsencesByIdAsync(string id);
        Task<Student?> GetStudentByNameAsync(string name, bool includeAbsences = false, bool includeClass = false);
        Task<IEnumerable<StudentDTO>> GetClassStudentsAsync(int classNumber, string servantId);
        public Task<int> GetClassCapacity(string classId);
        Task<string?> GetClassIdByStudentIdAsync(string studentId);
        Task<List<StudentCard?>> GetStudentCardsAsync();
        Task<bool> ExistsAsync(string id);
    }

}
