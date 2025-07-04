
using System.Text.Json.Serialization;
using DataFlow_ReadAPI.Services.DBFetching;
using Dbcheck;
using Scalar.AspNetCore;

namespace DataFlow_ReadAPI
{
    public partial class Program
    {
        

        public static async Task Main(string[] args)
        {
            bool Docker = false;

            var builder = WebApplication.CreateBuilder(args);

           
            foreach(var env in builder.Configuration.AsEnumerable())
            {
                if (env.Key.Contains("DCOMPOSE", StringComparison.OrdinalIgnoreCase)) Docker = true;
            }


            builder.Services.AddControllers()
                  .AddJsonOptions(options =>
                  {
                      options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                  }); ;

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddScoped<IDbFetch, DbFetch>();
            builder.Services.AddSingleton<Dbinit>();

            builder.Services.AddLogging(opt =>
            {
                opt.AddSimpleConsole(opt =>
                {
                    opt.TimestampFormat = "yyyy-MM-dd HH:mm:ss 'UTC' ";
                    opt.UseUtcTimestamp = true;
                    opt.SingleLine = true;
                });
            });



            var app = builder.Build();

            var dbInit = app.Services.GetRequiredService<Dbinit>();
            await dbInit.InitCreationAsync();

            //// Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.MapOpenApi();

            //}

           
            app.MapScalarApiReference("" ,opt =>
            {
            
                opt.WithTitle("TankForecast API");

                if(Docker) opt.AddServer("");
                else opt.AddServer("http://localhost:5051");
          

            });

           // app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }

    public partial class Program { }

}
