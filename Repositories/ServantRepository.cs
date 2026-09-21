using ManageData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyProject.Data;
using MyProject.Models.DTOs;
using MyProject.Models.Enums;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class ServantRepository : GenericRepository<IdentityUser>, IServantRepository
    {
        private readonly MadrasaDbContext _context;

        public ServantRepository(MadrasaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ServantDTO?> GetServantByPhoneNumberAsync(string phoneNumber)
        {
            //var servant = await _context.Users.SingleOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            var servant = await _context.Users.AsNoTracking()
                .Select(u => new ServantDTO
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    PhoneNumber = u.PhoneNumber,
                    Password = u.PasswordHash,
                    Role = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles,
                              ur => ur.RoleId,
                              r => r.Id,
                              (ur, r) => r.Name)
                        .FirstOrDefault() ?? "Servant"
                })
                .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
            return servant;
        }

        public async Task<string?> GetServantNameByEmailAsync(string email)
        {
            var servantName = await _context.Users.SingleOrDefaultAsync(u => u.Email == email);
            return servantName?.UserName;
        }

        public async Task AddClassSubjectsOfServantAsync(string servantId, Dictionary<int, int> servantClasses)
        {
            if (servantClasses == null || servantClasses.Count == 0)
                await Task.CompletedTask;
            foreach (var cs in servantClasses!)
            {
                var classId = await _context.Classes
                    .Where(c => c.Number == cs.Key)
                    .Select(c => c.Id)
                    .FirstOrDefaultAsync() ?? throw new InvalidOperationException($"Class {cs.Key} does not exist.");
                await _context.ServantClasses.AddAsync(new ServantClass
                {
                    Id = Guid.NewGuid().ToString(),
                    ClassId = classId,
                    Subject = (Subject)cs.Value,
                    ServantId = servantId
                });
            }
        }

        public async Task<bool> CheckServantPasswordAsync(string hashedPassword, string password)
        {
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var result = passwordHasher.VerifyHashedPassword(null, hashedPassword, password);
            return result == PasswordVerificationResult.Success;
        }

        public async Task<bool> ChangePassword(string email, string password)
        {
            var servant = await _context.Users.FirstOrDefaultAsync(o => o.Email == email);
            var hashPassword = new PasswordHasher<IdentityUser>();
            servant.PasswordHash = hashPassword.HashPassword(null, password);
            var result = _context.Users.Update(servant);
            return result != null;
        }
    }
}
