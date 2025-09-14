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
                . AddUserStore<UserStore<ApplicationUser,ApplicationRole,ApplicationDbContext,Guid>>()
                .AddRoleStore<RoleStore<ApplicationRole,ApplicationDbContext,Guid>>()   

                    ;
                var app = builder.Build();
                app.UseMiddleware<ErrorHandlingMiddleware>();

                //Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {''
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();
                // Appliquer la politique CORS avant l'authorization
                app.UseCors("AllowAngularDev");
                app.UseAuthentication();
                app.UseAuthorization();       
                app.MapControllers();
                app.Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur critique au d�marrage :{ex.Message}");
                Console.WriteLine("Erreur critique au d�marrage :");
                Console.WriteLine(ex.ToString()); // Affiche la stack compl�te dans la console
                throw;
            }
            finally
            {
                Log.CloseAndFlush(); // Fermeture propre gr�ce � Serilog.Extensions.Hosting
            }
        }
    }
}
