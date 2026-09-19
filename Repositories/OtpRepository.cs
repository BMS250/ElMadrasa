using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyProject.Data;
using MyProject.Models;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class OtpRepository : GenericRepository<OtpVerification>, IOtpRepository
    {
        private readonly MadrasaDbContext _context;

        public OtpRepository(MadrasaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CheckAndUseOtpAsync(string email, string otpCode)
        {
            var otp = await _context.Otps.FirstOrDefaultAsync(o => o.Email == email && o.OtpCode == otpCode);
            if (otp != null)
            {
                _context.Otps.Remove(otp);
                return true;
            }
            return false;
        }
    }
}
