namespace MyProject.Repositories.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAbsenceRepository Absences { get; }
        IStudentRepository Students { get; }
        IServantRepository Servants { get; }
        IOtpRepository Otps { get; }
        IClassRepository Classes { get; }
        IServantClassRepository ServantClasses { get; }
        IKorasAbsenceRepository KorasAbsences { get; }
        Task<int> CompleteAsync();
    }
}
