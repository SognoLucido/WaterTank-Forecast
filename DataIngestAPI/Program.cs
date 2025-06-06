
using DataIngestAPI.Services;
using Dbcheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace DataIngestAPI
{
    internal class Program
    {
        static async Task Main()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            builder.Services.AddLogging(opt =>
            {
                opt.AddSimpleConsole(opt =>
                {
                    opt.TimestampFormat = "yyyy-MM-dd HH:mm:ss 'UTC' ";
                    opt.UseUtcTimestamp = true;
                    opt.SingleLine = true;
                });
            });

            builder.Services.AddHostedService<MqttReaderBG>();
            builder.Services.AddSingleton<DbService>();

            builder.Services.AddSingleton<Dbinit>();

            IHost host = builder.Build();

           await host.RunAsync();
        }
    }
}
