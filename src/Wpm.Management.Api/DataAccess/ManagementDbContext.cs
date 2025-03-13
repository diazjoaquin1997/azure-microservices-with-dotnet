using Microsoft.EntityFrameworkCore;

namespace Wpm.Management.Api.DataAccess
{
    public class ManagementDbContext(DbContextOptions<ManagementDbContext> options) : DbContext(options)
    {
        public DbSet<Pet> Pets { get; set; }

        public DbSet<Breed> Breeds { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Breed>().HasData(
                [
                    new Breed(1, "Beagle"),
                    new Breed(2, "Caniche")
                ]
            );
            modelBuilder.Entity<Pet>().HasData(
                [
                    new Pet(){ Id=1, Name="Firulais", Age=11, BreedId=1 },
                    new Pet(){ Id=2, Name="Ron", Age=3, BreedId=2 },
                    new Pet(){ Id=3, Name="Wisky", Age=6, BreedId=1 },
                    new Pet(){ Id=4, Name="Chandom", Age=2, BreedId=2 }
                ]    
            );
        }
    }

    public static class ManageDbContextExtensions
    {
        public static void EnsureDbIsCreated(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetService<ManagementDbContext>();
            context!.Database.EnsureCreated();
        }
    }

    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }

        public int BreedId { get; set; }
        public Breed Breed { get; set; }
    }
    public record Breed(int Id, string Name);
}
