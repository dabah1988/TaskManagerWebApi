using Microsoft.EntityFrameworkCore;
using WebApiTaskManager.Core.Domain.Entities;

namespace TaskManager.Infrastructure.DatabaseContext
{
    public class ApplicationDbContext: DbContext
    {
       
        public DbSet<Project>? Projects { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
       : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            try
            {
                base.OnModelCreating(modelBuilder);
                modelBuilder.Entity<Project>().ToTable("Projects");
                // Remonte 4 niveaux depuis le dossier d'exécution vers la racine solution
                string path = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Data", "projects.json");
                string projectsJson = File.ReadAllText(path);
                List<Project>? projects = System.Text.Json.JsonSerializer.Deserialize<List<Project>>(projectsJson);
                // Initial data 
                if (projects != null)
                {
                    foreach (var project in projects)
                    {
                        modelBuilder.Entity<Project>().HasData(project);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
