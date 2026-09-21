using ManageData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using MyProject.Data;
using MyProject.Models.Enums;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class ServantClassRepository : GenericRepository<IdentityUser>, IServantClassRepository
    {
        private readonly MadrasaDbContext _context;
        private readonly IAbsenceRepository _absenceRepository;

        public ServantClassRepository(MadrasaDbContext context, IAbsenceRepository absenceRepository) : base(context)
        {
            _context = context;
            _absenceRepository = absenceRepository;
        }

        public Task<List<int>> GetClassesByServantIdAsync(string servantId)
        {
            return _context.ServantClasses.AsNoTracking()
                .Where(sc => sc.ServantId == servantId)
                .Select(sc => sc.Class.Number)
                .OrderBy(n => n)
                .ToListAsync();
        }

        public async Task<int> GetSubjectAsync(string servantId, string classId)
        {
            var result = await _context.ServantClasses.AsNoTracking()
                .FirstOrDefaultAsync(sc => sc.ServantId == servantId && sc.ClassId == classId);
            return result is null ? -1 : (int)result.Subject;
        }

        public async Task<Dictionary<int, bool>> CheckEftikadOfClassesAsync(List<int> servantClasses)
        {
            var classes = await _context.Classes
                .AsNoTracking()
                .Where(c => servantClasses.Contains(c.Number))
                .Select(c => new { c.Id, c.Number })
                .OrderBy(c => c.Number)
                .ToListAsync();

            //// Fire off all IsEftikadDone calls in parallel (ef core is not working with parallel calls)
            //var eftikadTasks = classes.Select(async c => new
            //{
            //    c.Number,
            //    IsDone = await _absenceRepository.IsEftikadDone(c.Id)
            //});

            //// Wait for them all to finish
            //var eftikadResults = await Task.WhenAll(eftikadTasks);

            //// Build your dictionary after all results are in
            //return eftikadResults.ToDictionary(x => x.Number, x => x.IsDone);
            var result = new Dictionary<int, bool>();
            //var lastDate = await _absenceRepository.GetLastAbsenceDateAsync();
            var lastDate = await _context.Absences
                .MaxAsync(a => a.AbsenceDate);
            if (lastDate == default) return result;
            foreach (var c in classes)
            {
                //var isDone = await _absenceRepository.IsEftikadDone(c.Id, lastDate);
                if (lastDate == default) result[c.Number] = false;
                var classStudents = await _context.Students
                    .Where(s => s.ClassId == c.Id && s.State == State.عادى)
                    .Select(s => s.Id)
                    .ToListAsync();
                var anyAttendant = await _context.Absences
                    .AnyAsync(a => a.AbsenceDate == lastDate &&
                                    classStudents.Contains(a.StudentId) &&
                                    (!a.AlhanAttendant || !a.CopticAttendant || !a.TacsAttendant) &&
                                    string.IsNullOrWhiteSpace(a.AbsenceReason));
                var isDone = !anyAttendant;
                result[c.Number] = isDone;
            }

            return result;

        }
    }
}
