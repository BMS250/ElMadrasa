using ManageData.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyProject.Controllers;
using MyProject.Data;
using MyProject.Models;
using MyProject.Repositories;
using MyProject.Repositories.IRepositories;
using MyProject.S3Services;
using MyProject.Services;
using MyProject.Services.IServices;
using Newtonsoft.Json;
using QuestPDF.Infrastructure;
namespace MyProject
{
    public class Program
    {
        public static void /*async Task*/ Main(string[] args)
        {
            //var builder = WebApplication.CreateBuilder(new WebApplicationOptions
            //{
            //    Args = args,
            //    EnvironmentName = Environments.Production // Explicitly set to Production
            //});
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddDbContext<MadrasaDbContext>(options =>
            {
                try
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Database connection failed: " + ex.Message);
                }
            });

            builder.Services.AddIdentity<IdentityUser, IdentityRole>() // notice IdentityRole
            .AddEntityFrameworkStores<MadrasaDbContext>()
            .AddDefaultTokenProviders();


            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.SetMinimumLevel(LogLevel.Debug);

            builder.Services.AddControllers()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });
            builder.Services.AddEndpointsApiExplorer();

            //builder.Services.AddHostedService<Scheduling>();
            builder.Services.AddMemoryCache();
            // Add S3 Service
            builder.Services.AddSingleton<S3Service>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IServantRepository, ServantRepository>();
            builder.Services.AddScoped<IStudentRepository, StudentRepository>();
            builder.Services.AddScoped<IAbsenceRepository, AbsenceRepository>();
            builder.Services.AddScoped<IOtpRepository, OtpRepository>();
            builder.Services.AddScoped<IServantClassRepository, ServantClassRepository>();
            builder.Services.AddScoped<IClassRepository, ClassRepository>();
            builder.Services.AddScoped<IKorasAbsenceRepository, KorasAbsenceRepository>();
            builder.Services.AddScoped<IClassService, ClassService>();
            builder.Services.AddScoped<IServantService, ServantService>();
            builder.Services.AddScoped<StudentCardPdfService>();

            // Add Controllers
            builder.Services.AddControllers();


            QuestPDF.Settings.License = LicenseType.Evaluation;

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Seed Roles
            //using (var scope = app.Services.CreateScope())
            //{
            //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //    await IdentitySeeder.SeedRolesAsync(roleManager);
            //}


            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
        

    }
}

