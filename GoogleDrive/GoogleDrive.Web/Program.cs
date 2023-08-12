
using GoogleDrive.Database;
using Microsoft.EntityFrameworkCore;

namespace GoogleDrive.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var configuration = builder.Configuration;
            // Add services to the container.
            builder.Services.AddDbContext<GoogleDriveDbContext>(options =>
                {
                    options.UseSqlServer(configuration.GetConnectionString("GoogleDriveConnectionString"),
                        sqlServerOptionsAction =>
                        {
                            sqlServerOptionsAction.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                        });
                }
            );


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}