using Microsoft.EntityFrameworkCore;
using MyProject.Data;
using MyProject.Migrations;
using MyProject.Models;
using MyProject.Models.DTOs;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class StudentRepository : GenericRepository<Student>, IStudentRepository
    {
        private readonly MadrasaDbContext _context;
        private readonly IServantClassRepository _servantClassesRepository;
        public StudentRepository(MadrasaDbContext context, IServantClassRepository servantClassesRepository) : base(context)
        {
            _context = context;
            _servantClassesRepository = servantClassesRepository;
        }

        public async Task<IEnumerable<StudentDTO>> GetAllWithAbsencesAsync()
        {
            var students = await _context.Students
                .Include(s => s.Absences.OrderBy(a => a.AbsenceDate))
                .Include(s => s.Class)
                .ToListAsync();
            var studentDTOs = students.Select(s => new StudentDTO
            {
                Id = s.Id,
                Name = s.Name,
                State = s.State,
                Class = s.Class.Number,
                Age = s.Age,
                MamPhone = s.MamPhone,
                DadPhone = s.DadPhone,
                StudPhone = s.StudPhone,
                BirthDate = s.BirthDate,
                Gender = s.Gender,
                ClassId = s.ClassId,
                Notes = s.Notes,
                NumberOfAbsences = s.NumberOfAbsences,
                ProfileImage = s.ProfileImage,
                Absences = s.Absences
            });
            return studentDTOs;
        }

        public async Task<StudentDTO?> GetStudentWithAbsencesByIdAsync(string id)
        {
            var s = await _context.Students
                .Include(s => s.Absences)
                .Include(s => s.Class)
                .FirstOrDefaultAsync(s => s.Id == id);
            var studentDTO = new StudentDTO
            {
                Id = s.Id,
                Name = s.Name,
                State = s.State,
                Class = s.Class.Number,
                Age = s.Age,
                MamPhone = s.MamPhone,
                DadPhone = s.DadPhone,
                StudPhone = s.StudPhone,
                BirthDate = s.BirthDate,
                Gender = s.Gender,
                ClassId = s.ClassId,
                Notes = s.Notes,
                NumberOfAbsences = s.NumberOfAbsences,
                ProfileImage = s.ProfileImage,
                Absences = s.Absences
            };
            return studentDTO;
        }

        public async Task<Student?> GetStudentByNameAsync(string name, bool includeAbsences = false, bool includeClass = false)
        {
            List<Student> query;
            if (includeAbsences)
            {
                if (includeClass)
                {
                    query = await _context.Students
                    .Include(s => s.Absences)
                    .Include(s => s.Class)
                    .ToListAsync();
                }
                else
                {
                    query = await _context.Students
                        .Include(s => s.Absences)
                        .ToListAsync();
                }
            }
            else
            {
                if (includeClass)
                {
                    query = await _context.Students
                    .Include(s => s.Class)
                    .ToListAsync();
                }
                else
                {
                    query = await _context.Students
                        .ToListAsync();
                }
            }
            return query.FirstOrDefault(s => s.Name == name);
        }
        public async Task<IEnumerable<StudentDTO>> GetClassStudentsAsync(int classNumber, string servantId)
        {
            bool? lastAttendance = true;
            var result = await _context.Students
                .Include(s => s.Absences)
                .Where(s => s.Class.Number == classNumber && s.State == State.عادى)
                .Select(s => new StudentDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    State = s.State,
                    Class = s.Class.Number,
                    Age = s.Age,
                    MamPhone = s.MamPhone,
                    DadPhone = s.DadPhone,
                    StudPhone = s.StudPhone,
                    BirthDate = s.BirthDate,
                    Gender = s.Gender,
                    ClassId = s.ClassId,
                    Notes = s.Notes,
                    NumberOfAbsences = s.NumberOfAbsences,
                    ProfileImage = s.ProfileImage,
                    //Absences = s.Absences
                    Absences = s.Absences
                    .OrderBy(a => a.AbsenceDate)
                    .Select(a => new Absence
                    {
                        Id = a.Id,
                        StudentId = a.StudentId,
                        TacsAttendant = a.TacsAttendant,
                        AlhanAttendant = a.AlhanAttendant,
                        CopticAttendant = a.CopticAttendant,
                        AbsenceDate = a.AbsenceDate,
                        AbsenceReason = a.AbsenceReason,
                        Student = null
                    }).ToList()
                })
                .ToListAsync();
            //var classId = await _context.Classes
            //    .Where(c => c.Number == classNumber)
            //    .Select(c => c.Id)
            //    .FirstOrDefaultAsync();
            var classId = result.FirstOrDefault()?.ClassId;
            if (classId == null)
                return new List<StudentDTO>();
            var subject = await _servantClassesRepository.GetSubjectAsync(servantId, classId);
            result.ForEach(s =>
            {
                //var latestAbsence = s.Absences
                //    .OrderByDescending(a => a.AbsenceDate)
                //    .FirstOrDefault();
                var latestAbsence = s.Absences
                    .MaxBy(a => a.AbsenceDate);
                if (latestAbsence != null)
                {
                    if (subject == 0)
                    {
                        lastAttendance = latestAbsence.AlhanAttendant;
                    }
                    else if (subject == 1)
                    {
                        lastAttendance = latestAbsence.CopticAttendant;
                    }
                    else if (subject == 2)
                    {
                        lastAttendance = latestAbsence.TacsAttendant;
                    }
                }
                else
                {
                    lastAttendance = true;
                }
                s.LastAttendance = lastAttendance;
            });
            result = [.. result.OrderBy(r => r.Name)];
            return result;
        }
        public Task<int> GetClassCapacity(string classId)
        {
            return _context.Students.Where(s => s.State == State.عادى).CountAsync(s => s.ClassId == classId);
        }

        public Task<string?> GetClassIdByStudentIdAsync(string studentId)
        {
            return _context.Students
                .Where(s => s.Id == studentId)
                .Select(s => s.ClassId)
                .FirstOrDefaultAsync();

        }

        public async Task<List<StudentCard?>> GetStudentCardsAsync()
        {
            var students = await _context.Students
                .Include(s => s.Class)
                .ToListAsync();
            var studentCards = students.Select(s => new StudentCard
            {
                Id = s.Id,
                Name = s.Name,
                Level = (Level)s.Class.Level,
                ClassNumber = s.Class.Number
            }).ToList();
            return studentCards;
        }

        public async Task<bool> ExistsAsync(string id)
        {
            return await _context.Students.AnyAsync(s => s.Id == id);
        }
    }
}
