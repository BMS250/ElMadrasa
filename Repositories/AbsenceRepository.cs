using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Models;
using MyProject.Models.DTOs;
using MyProject.Models.Enums;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class AbsenceRepository : GenericRepository<Absence>, IAbsenceRepository
    {
        private readonly MadrasaDbContext _context;

        public AbsenceRepository(MadrasaDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Absence>> GetAbsencesWithStudentsAsync()
        {
            return await _context.Absences.Include(a => a.Student).ToListAsync();
        }

        public async Task<IEnumerable<FullAbsenceDTO>> GetAbsencesWithStudentsAndServantIdAsync(string servantId)
        {
            var absences = await _context.Absences.Include(a => a.Student).ToListAsync();
            var result = new List<FullAbsenceDTO>();
            foreach (var absence in absences)
            {
                var servantClass = await _context.ServantClasses.AsNoTracking()
                    .FirstOrDefaultAsync(sc => sc.ServantId == servantId && sc.ClassId == absence.Student.ClassId);
                var subject = servantClass is null ? -1 : (int)servantClass.Subject;

                bool? lastAbsences = true;
                if (subject == 0)
                {
                    lastAbsences = absence.AlhanAttendant;
                }
                else if (subject == 1)
                {
                    lastAbsences = absence.CopticAttendant;
                }
                else if (subject == 2)
                {
                    lastAbsences = absence.TacsAttendant;
                }
                result.Add(new FullAbsenceDTO
                {
                    Id = absence.Id,
                    StudentId = absence.StudentId,
                    AbsenceDate = absence.AbsenceDate,
                    AbsenceReason = absence.AbsenceReason,
                    Student = absence.Student,
                    AlhanAttendant = absence.AlhanAttendant,
                    CopticAttendant = absence.CopticAttendant,
                    TacsAttendant = absence.TacsAttendant,
                    LastAttendance = lastAbsences
                });
            }
            result = [.. result.OrderBy(r => r.Student.Name)];
            return result;
        }

        public Task<Absence?> GetAbsenceWithStudentAsync(string id)
        {
            return _context.Absences
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Absence>> GetStudentAbsencesByIdAsync(string studentId)
        {
            return await _context.Absences
                .Include(a => a.Student)
                .Where(a => a.StudentId == studentId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Absence>> GetStudentAbsencesByNameAsync(string studentName)
        {
            var student = await _context.Students
                .SingleOrDefaultAsync(s => s.Name == studentName);

            if (student == null) return new List<Absence>();

            return await _context.Absences
                .Include(a => a.Student)
                .Where(a => a.StudentId == student.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<SubjectAbsenceDTO>> GetClassWithAbsenceStatusAsync(int classNumber)
        {
            var lastDate = await GetLastAbsenceDateAsync();

            var students = await _context.Students
                .Include(s => s.Absences)
                .Where(s => s.Class.Number == classNumber && s.State == State.عادى)
                .ToListAsync();

            var studentsAbsences = students
                .Where(student => student.Absences.Any(a => a.AbsenceDate == lastDate && !a.Attendant))
                .Select(student =>
                {
                    var absence = student.Absences.First(a => a.AbsenceDate == lastDate && !a.Attendant);
                    return new SubjectAbsenceDTO
                    {
                        Student = student,
                        AlhanAbsence = absence.AlhanAttendant,
                        CopticAbsence = absence.CopticAttendant,
                        TacsAbsence = absence.TacsAttendant
                    };
                })
                .ToList();

            return studentsAbsences;
        }


        public async Task<bool> IsEftikadDone(string classId, DateOnly lastDate)
        {
            if (lastDate == default) return false;
            var classStudents = await _context.Students
                .Where(s => s.ClassId == classId && s.State == State.عادى)
                .Select(s => s.Id)
                .ToListAsync();
            var result = await _context.Absences
                .AnyAsync(a => a.AbsenceDate == lastDate && 
                                classStudents.Contains(a.StudentId) && 
                                (!a.AlhanAttendant || !a.CopticAttendant || !a.TacsAttendant) && 
                                string.IsNullOrWhiteSpace(a.AbsenceReason));
            return !result;
        }

        public Task<DateOnly> GetLastAbsenceDateAsync()
        {
            return _context.Absences
                .MaxAsync(a => a.AbsenceDate);
        }

        public async Task<int> GetAttendantsCountAsync(string classId)
        {
            var classStudents = _context.Students
                .Where(s => s.ClassId == classId && s.State == State.عادى)
                .Select(s => s.Id);
            var lastDate = await GetLastAbsenceDateAsync();
            return await _context.Absences
                .CountAsync(a => a.AbsenceDate == lastDate && classStudents.Contains(a.StudentId) && a.AlhanAttendant && a.CopticAttendant && a.TacsAttendant);
        }
    }
}
