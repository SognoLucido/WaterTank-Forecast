
using System.IO;
using System.Text.Json.Serialization;
using DataFlow_ReadAPI.Services.DBFetching;
using Microsoft.AspNetCore.Http.Json;
using Scalar.AspNetCore;

namespace DataFlow_ReadAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);



            builder.Services.AddControllers()
                  .AddJsonOptions(options =>
                  {
                      options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                  }); ;

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IDbFetch, DbFetch>();



           

            var app = builder.Build();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
                app.MapOpenApi();

            //}

           
            app.MapScalarApiReference("/" ,opt =>
            {
            
                opt.WithTitle("TankForecast API");
                opt.AddServer(""); //  docker 
                opt.AddServer("http://localhost:5051"); // no docker , dbconnection manual edit req *** TODO dynamic 
                
            });

           // app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
