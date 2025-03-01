using System.Collections.Concurrent;
using BD_Assignment_2025.BackgroundJobs;
using BD_Assignment_2025.Entities;
using BD_Assignment_2025.IRepositories;
using BD_Assignment_2025.IServices;
using BD_Assignment_2025.Middlewares;
using BD_Assignment_2025.Repositories;
using BD_Assignment_2025.Services;

namespace BD_Assignment_2025
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpContextAccessor();
            builder.Services.RegisterServices();

            var app = builder.Build();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseMiddleware<ExceptionHandler>();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.RegisterAllEndpoints();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
