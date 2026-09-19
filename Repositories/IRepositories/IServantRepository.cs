using Microsoft.AspNetCore.Identity;
using MyProject.Models.DTOs;

namespace MyProject.Repositories.IRepositories
{
    public interface IServantRepository : IGenericRepository<IdentityUser>
    {
        Task<ServantDTO?> GetServantByPhoneNumberAsync(string phoneNumber);
        Task<string?> GetServantNameByEmailAsync(string email);
        Task AddClassSubjectsOfServantAsync(string servantId, Dictionary<int, int> servantClasses);
        Task<bool> CheckServantPasswordAsync(string phoneNumber, string password);
        Task<bool> ChangePassword(string email, string password);
    }

}

