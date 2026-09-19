using MyProject.Models;

namespace MyProject.Repositories.IRepositories
{
    public interface IKorasAbsenceRepository
    {
        Task InsertKorasAbsence(string studentId);
    }

}

