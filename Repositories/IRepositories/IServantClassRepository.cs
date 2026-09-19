using Microsoft.AspNetCore.Identity;
using MyProject.Models;

namespace MyProject.Repositories.IRepositories
{
    public interface IServantClassRepository : IGenericRepository<IdentityUser>
    {
        Task<List<int>> GetClassesByServantIdAsync(string servantId);
        public Task<int> GetSubjectAsync(string servantId, string classId);
        Task<Dictionary<int, bool>> CheckEftikadOfClassesAsync(List<int> servantClasses);
    }
}

