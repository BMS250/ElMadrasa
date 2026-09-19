using MyProject.Models;
using MyProject.Models.DTOs;

namespace MyProject.Repositories.IRepositories
{
    public interface IAbsenceRepository : IGenericRepository<Absence>
    {
        Task<IEnumerable<Absence>> GetAbsencesWithStudentsAsync();
        public Task<IEnumerable<FullAbsenceDTO>> GetAbsencesWithStudentsAndServantIdAsync(string servantId);
        Task<Absence?> GetAbsenceWithStudentAsync(string id);
        Task<IEnumerable<Absence>> GetStudentAbsencesByIdAsync(string studentId);
        Task<IEnumerable<Absence>> GetStudentAbsencesByNameAsync(string studentName);
        Task<IEnumerable<SubjectAbsenceDTO>> GetClassWithAbsenceStatusAsync(int classNumber);
        Task<bool> IsEftikadDone(string classId, DateOnly lastDate);
        Task<DateOnly> GetLastAbsenceDateAsync();
        Task<int> GetAttendantsCountAsync(string classId);
    }

}

