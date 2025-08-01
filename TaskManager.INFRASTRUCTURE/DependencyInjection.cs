﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManager.Core.Domain.RepositoryContracts;
using TaskManager.Core.Services;
using TaskManager.Core.ServicesContract;
using TaskManager.Infrastructure.DatabaseContext;
using TaskManager.Infrastructure.Repository;

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
