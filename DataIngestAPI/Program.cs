using System.Threading.Tasks;
using DataIngestAPI.Services;
using Dbcheck;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataIngestAPI
{
    internal class Program
    {
        static async Task Main()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();

            builder.Services.AddHostedService<MqttReaderBG>();
            builder.Services.AddSingleton<DbService>();

            builder.Services.AddSingleton<Dbinit>();

            IHost host = builder.Build();

           await host.RunAsync();
        }
    }
}
