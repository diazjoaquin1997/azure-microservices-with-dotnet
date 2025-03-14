using Microsoft.EntityFrameworkCore;
using Wpm.Clinic.DataAccess;

namespace Wpm.Clinic.DataAccess
{
    public class ClinicDbContext(DbContextOptions<ClinicDbContext> options) : DbContext(options)
    {
        public DbSet<Consultation> Consultations { get; set; }
    }
}

public record Consultation(Guid Id, int PatientId, string PatientName, int PatientAge, DateTime StartTime);

public static class ClinicDbContextExtensions
{
    public static void EnsureClinicDbIsCreated(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetService<ClinicDbContext>();
        context!.Database.EnsureCreated();
    }
}