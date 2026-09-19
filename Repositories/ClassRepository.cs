using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;
using MyProject.Models.DTOs;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class ClassRepository : GenericRepository<IdentityUser>, IClassRepository
    {
        private readonly MadrasaDbContext _context;
        private readonly IAbsenceRepository _absenceRepository;
        private readonly IStudentRepository _studentRepository;

        public ClassRepository(MadrasaDbContext context, IAbsenceRepository absenceRepository, IStudentRepository studentRepository) : base(context)
        {
            _context = context;
            _absenceRepository = absenceRepository;
            _studentRepository = studentRepository;
        }

        public Task<List<Class>> GetAllAsync()
        {
            return _context.Classes.AsNoTracking().ToListAsync();
        }

        public Task<List<int>> GetAllNumbersAsync()
        {
            return _context.Classes.AsNoTracking().Select(c => c.Number).ToListAsync();
        }

        public Task<List<SummarizedClass>> GetAllSummarizedAsync()
        {
            return _context.Classes.AsNoTracking()
                .Select(c => new SummarizedClass
                {
                    Id = c.Id,
                    Number = c.Number
                })
                .ToListAsync();
        }

        public Task<SummarizedClass?> GetSummarizedClassAsync(string id)
        {
            return _context.Classes.AsNoTracking()
                .Select(c => new SummarizedClass
                {
                    Id = c.Id,
                    Number = c.Number
                })
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // I don't know why GetByIdAsync from GenericRepository doesn't work here
        public Task<Class?> GetById(string id)
        {
            return _context.Classes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<ClassCapacityDTO?> GetCapacityByNumberAsync(int classNumber)
        {
            // 1️⃣ Query class info first
            var classInfo = await _context.Classes
                .Where(c => c.Number == classNumber)
                .Select(c => new { c.Id, c.Number })
                .FirstOrDefaultAsync();

            if (classInfo == null)
                return null;

            // 2️⃣ Then call repositories separately (await each)
            var capacity = await _studentRepository.GetClassCapacity(classInfo.Id);
            var attendantsCount = await _absenceRepository.GetAttendantsCountAsync(classInfo.Id);

            // 3️⃣ Return DTO
            return new ClassCapacityDTO
            {
                ClassNumber = classInfo.Number,
                Capacity = capacity,
                NumberOfAttendants = attendantsCount
            };
        }


        public async Task<int> GetClassNumberById(string id)
        {
            var Class = await _context.Classes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
            return Class?.Number ?? -1;
        }

        public async Task<string?> GetClassIdByNumber(int number)
        {
            var Class = await _context.Classes.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Number == number);
            return Class?.Id;
        }
    }
}
