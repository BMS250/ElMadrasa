using ManageData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProject.Models;

namespace MyProject.Data
{
    public class MadrasaDbContext : IdentityDbContext<IdentityUser>
    //public class MadrasaDbContext : DbContext
    {
        public MadrasaDbContext(DbContextOptions<MadrasaDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Absence> Absences { get; set; }
        public virtual DbSet<Class> Classes { get; set; }
        public virtual DbSet<ServantClass> ServantClasses { get; set; }
        public virtual DbSet<OtpVerification> Otps { get; set; }
        public virtual DbSet<KorasAbsence> KorasAbsences { get; set; }
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //// Configure relationships
            modelBuilder.Entity<ServantClass>()
                .HasOne(sc => sc.Servant)
                .WithMany()
                .HasForeignKey(sc => sc.ServantId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServantClass>()
                .HasOne(sc => sc.Class)
                .WithMany(c => c.ServantClasses)
                .HasForeignKey(sc => sc.ClassId);

            modelBuilder.Entity<Class>()
                .HasIndex(c => c.Number)
                .IsUnique();
        }
    }
}
