using Microsoft.AspNetCore.Mvc;
using MyProject.Models;

namespace MyProject.Services.IServices
{
    public interface IServantService
    {
        Task<List<int>?> GetServantClasses(string servantId, string role);
        bool IsPhoneNumberValid(string phoneNumber);
    }

}
