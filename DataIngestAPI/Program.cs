using DataIngestAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DataIngestAPI
{
    internal class Program
    {
        static void Main()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
            builder.Services.AddHostedService<MqttReaderBG>();

            builder.Services.AddSingleton<DbService>();

            IHost host = builder.Build();
            host.Run();
        }
    }
}
