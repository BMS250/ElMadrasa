using Microsoft.AspNetCore.Identity;
using MyProject.Models;
using MyProject.Models.DTOs;

namespace MyProject.Repositories.IRepositories
{
    public interface IClassRepository : IGenericRepository<IdentityUser>
    {
        Task<List<Class>> GetAllAsync();
        Task<List<int>> GetAllNumbersAsync();
        Task<List<SummarizedClass>> GetAllSummarizedAsync();
        Task<SummarizedClass?> GetSummarizedClassAsync(string id);
        Task<Class?> GetById(string id);
        Task<ClassCapacityDTO?> GetCapacityByNumberAsync(int classNumber);
        Task<int> GetClassNumberById(string id);
        public Task<string?> GetClassIdByNumber(int number);
    }
}
