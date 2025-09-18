using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Serilog;
using TaskManager.Core.Identity;
using TaskManager.Infrastructure;
using TaskManager.Infrastructure.DatabaseContext;
using TaskManager.UI.Exceptions;
using TaskManager.UI.Utilities;


namespace WebApiTaskManager
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {

                var builder = WebApplication.CreateBuilder(args);
                var angularUrl = builder.Configuration["AngularClient:Url"];
                builder.Services.Configure<paginationSettings>(builder.Configuration.GetSection("NumberElementsByPage"));

                if (!string.IsNullOrWhiteSpace(angularUrl))
                {
                    builder.Services.AddCors(options =>
                    {
                        options.AddPolicy("AllowAngularDev",
                            policy =>
                            {
                                policy.WithOrigins(angularUrl) // l�URL de ton Angular
                                      .AllowAnyHeader()
                                      .AllowAnyMethod();
                            });
                    });
                }



                // R�cup�rer la configuration compl�te
                var configuration = builder.Configuration;

                // Configurer Serilog en lisant la config dans appsettings.json
                Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(configuration)
                    .CreateLogger();

                // Injecter Serilog dans le pipeline de hosting
                builder.Host.UseSerilog();
                // Add services to the container.

                builder.Services.AddControllers();
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();
                builder.Services.AddInfrastructure(builder.Configuration);
                builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequiredLength = 6;
                })
                  .AddEntityFrameworkStores<ApplicationDbContext>()
                  .AddDefaultTokenProviders()
                .AddUserStore<UserStore<ApplicationUser, ApplicationRole, ApplicationDbContext, Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole, ApplicationDbContext, Guid>>();



                var app = builder.Build();
                app.UseMiddleware<ErrorHandlingMiddleware>();

                //Configure the HTTP request pipeline.
                if (!builder.Environment.ApplicationName.Contains("ef"))
                {
                    app.UseMiddleware<ErrorHandlingMiddleware>();

                    if (app.Environment.IsDevelopment())
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    app.UseHttpsRedirection();
                    app.UseStaticFiles();
                    app.UseRouting();
                    app.UseCors("AllowAngularDev");
                    app.UseAuthentication();
                    app.UseAuthorization();
                    app.MapControllers();
                    app.Run();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("❌ Erreur critique au démarrage !");
                Console.ResetColor();

                Console.WriteLine($"Message : {ex.Message}");

                // Parcours des InnerExceptions pour afficher la vraie cause
                var inner = ex;
                while (inner.InnerException != null)
                {
                    inner = inner.InnerException;
                    Console.WriteLine($"➡️ InnerException : {inner.Message}");
                }

                Console.WriteLine("StackTrace :");
                Console.WriteLine(ex.ToString());

                throw; // important pour que le process s’arrête proprement
            }

            finally
            {
                Log.CloseAndFlush(); // Fermeture propre gr�ce � Serilog.Extensions.Hosting
            }
        }
    }
}
