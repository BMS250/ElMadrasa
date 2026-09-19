using MyProject.Data;
using MyProject.Repositories.IRepositories;

namespace MyProject.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MadrasaDbContext _context;
        public IAbsenceRepository Absences { get; }
        public IStudentRepository Students { get; }
        public IServantRepository Servants { get; }
        public IOtpRepository Otps { get; }
        public IClassRepository Classes { get; }
        public IServantClassRepository ServantClasses { get; }
        public IKorasAbsenceRepository KorasAbsences { get; }

        public UnitOfWork(
            MadrasaDbContext context,
            IAbsenceRepository absenceRepository,
            IStudentRepository studentRepository,
            IServantRepository servantRepository,
            IOtpRepository otpRepository,
            IClassRepository classRepository,
            IServantClassRepository servantClasses,
            IKorasAbsenceRepository korasAbsenceRepository)
        {
            _context = context;
            Absences = absenceRepository;
            Students = studentRepository;
            Servants = servantRepository;
            Otps = otpRepository;
            Classes = classRepository;
            ServantClasses = servantClasses;
            KorasAbsences = korasAbsenceRepository;
        }

        public async Task<int> CompleteAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }

}
