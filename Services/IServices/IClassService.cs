using MyProject.Models;
using MyProject.Models.DTOs;

namespace MyProject.Services.IServices
{
    public interface IClassService
    {
        Task<List<Class>> GetAllAsync();
        Task<List<SummarizedClass>> GetAllSummarizedAsync();
        Task<SummarizedClass?> GetSummarizedClassAsync(string id);
        Task<Class?> GetByIdAsync(string id);
        void InvalidateCache();
    }

}
