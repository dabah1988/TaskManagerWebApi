using Serilog;
using TaskManager.Infrastructure;


namespace WebApiTaskManager
{
    public class Program
    {
        public static void Main(string[] args)
        {

            try
            {
                var builder = WebApplication.CreateBuilder(args);

   

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
                var app = builder.Build();

                //Configure the HTTP request pipeline.
                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseStaticFiles();
                app.UseHttpsRedirection();

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
