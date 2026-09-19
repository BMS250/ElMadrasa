using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MyProject.Models;
using MyProject.Repositories.IRepositories;
using MyProject.Services.IServices;
using System.Text.Json;

namespace MyProject.Services
{
    public class ServantService : IServantService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServantService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<int>?> GetServantClasses(string servantId, string role)
        {
            if (string.IsNullOrEmpty(servantId))
            {
                return null;
            }
            List<int> classes;
            if (role == "Admin")
            {
                classes = await _unitOfWork.Classes.GetAllNumbersAsync();
            }
            else
            {
                classes = await _unitOfWork.ServantClasses.GetClassesByServantIdAsync(servantId);
            }
            if (classes == null)
            {
                return new List<int>();
            }
            return [.. classes.Distinct()];
        }

        public bool IsPhoneNumberValid(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return false;
            }
            if ((phoneNumber.Length == 11 && phoneNumber.StartsWith("01")) ||
                (phoneNumber.Length == 10 && phoneNumber.StartsWith("1")))
            {
                foreach (char c in phoneNumber)
                {
                    if (!char.IsDigit(c))
                    {
                        return false;
                    }
                }
                return true;
            }
            
            return false;
        }
    }

}
