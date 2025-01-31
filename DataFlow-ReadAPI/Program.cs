
using DataFlow_ReadAPI.Services.DBFetching;
using Scalar.AspNetCore;

namespace DataFlow_ReadAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IDbFetch, DbFetch>();

            var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.MapOpenApi();
            //}

            app.MapScalarApiReference(opt =>
            {
                opt.WithTitle("TankForecast API");
                opt.AddServer("http://localhost:5051");
            });

           // app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
