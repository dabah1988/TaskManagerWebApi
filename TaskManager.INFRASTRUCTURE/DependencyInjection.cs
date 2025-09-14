using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.Identity;
using TaskManager.Core.Services;
using TaskManager.Core.ServicesContract;
using TaskManager.Infrastructure.DatabaseContext;
using TaskManager.Infrastructure.Repository;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;


namespace TaskManager.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
       
            services.AddDbContext<ApplicationDbContext>(
                options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("TaskManager"));
                }
                );
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<ITaskManagerRepository, TaskManagerRepository>();
            services.AddScoped<ITaskManagerService, TaskManagerService>();
         
            return services;
        }
      
    }
}
