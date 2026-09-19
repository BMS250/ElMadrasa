using MyProject.Models;

namespace MyProject.Repositories.IRepositories
{
    public interface IOtpRepository : IGenericRepository<OtpVerification>
    {
        Task<bool> CheckAndUseOtpAsync(string email, string otp);
    }

}

