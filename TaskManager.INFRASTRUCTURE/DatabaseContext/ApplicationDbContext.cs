using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Identity;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser,ApplicationRole,Guid>
    {

        // EF va utiliser ce constructeur si jamais il ne trouve pas d’options
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=TaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }
        public DbSet<Project>? Projects { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Project>().ToTable("Projects");
            try
            {
                // Remonte 4 niveaux depuis le dossier d'exécution vers la racine solution
                //string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data", "projects.json");
                //if (File.Exists(path))
                //{
                //    string projectsJson = File.ReadAllText(path);
                //    List<Project>? projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(projectsJson);
                //    // Initial data 
                //    if (projects != null)
                //    {
                //        foreach (var project in projects)
                //        {
                //            modelBuilder.Entity<Project>().HasData(project);
                //        }
                //    }
                //}
                //else
                //{
                //}
               
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
