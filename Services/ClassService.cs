using Microsoft.Extensions.Caching.Memory;
using MyProject.Models;
using MyProject.Models.DTOs;
using MyProject.Repositories.IRepositories;
using MyProject.Services.IServices;

namespace MyProject.Services
{
    public class ClassService : IClassService
    {
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;
        private const string CacheKey = "AllClasses";

        public ClassService(IMemoryCache cache, IUnitOfWork unitOfWork)
        {
            _cache = cache;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Class>> GetAllAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<Class>? classes))
            {
                classes = await _unitOfWork.Classes.GetAllAsync();

                _cache.Set(CacheKey, classes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                });
            }

            return classes!;
        }

        public async Task<List<SummarizedClass>> GetAllSummarizedAsync()
        {
            if (!_cache.TryGetValue(CacheKey, out List<SummarizedClass>? classes))
            {
                classes = await _unitOfWork.Classes.GetAllSummarizedAsync();

                _cache.Set(CacheKey, classes, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12)
                });
            }

            return classes!;
        }

        public Task<SummarizedClass?> GetSummarizedClassAsync(string id)
        {
            return _unitOfWork.Classes.GetSummarizedClassAsync(id);
        }

        public async Task<Class?> GetByIdAsync(string id)
        {
            var classes = await GetAllAsync();
            return classes.FirstOrDefault(c => c.Id == id);
        }

        public void InvalidateCache()
        {
            _cache.Remove(CacheKey);
        }
    }

}
