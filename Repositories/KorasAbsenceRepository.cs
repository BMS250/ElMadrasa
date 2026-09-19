using MyProject.Data;
using MyProject.Models;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories.IRepositories
{
    public class KorasAbsenceRepository : IKorasAbsenceRepository
    {
        private readonly MadrasaDbContext _context;
        public KorasAbsenceRepository(MadrasaDbContext context)
        {
            _context = context;
        }

        public async Task InsertKorasAbsence(string studentId)
        {
            var absence = new KorasAbsence
            {
                Id = Guid.NewGuid().ToString(),
                StudentId = studentId,
                AbsenceDate = DateOnly.FromDateTime(DateTime.Now),
                Attendant = true
            };
            _context.KorasAbsences.Add(absence);
            await _context.SaveChangesAsync();
        }
    }

}

