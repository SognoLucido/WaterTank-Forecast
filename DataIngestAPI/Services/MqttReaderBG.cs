using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MQTTnet.Formatter;
using MQTTnet;

namespace DataIngestAPI.Services
{
    internal class MqttReaderBG(DbService db , IConfiguration conf) : BackgroundService
    {
        private readonly DbService dbcall = db;
        private string MqttServerIP = conf["DCOMPOSE_MQTTBROKERHOST"] ?? conf.GetConnectionString("mqtt") ?? "localhost"  ;

        protected override async Task ExecuteAsync(CancellationToken ctoken)
        {
            //while (!stoppingToken.IsCancellationRequested)
            //{
            await dbcall.InitCreation();
            var mqttFactory = new MqttClientFactory();

                using var mqttClient = mqttFactory.CreateMqttClient();

                var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(MqttServerIP)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .WithKeepAlivePeriod(TimeSpan.FromMinutes(5))
                .Build();

            var response = await mqttClient.ConnectAsync(mqttClientOptions, ctoken);

            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var test = e.ApplicationMessage.UserProperties;


                //Console.WriteLine($"Topic : {e.ApplicationMessage.Topic} and Message : {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}"); 

              await dbcall.Insertdata(Encoding.UTF8.GetString(
                  e.ApplicationMessage.Payload),
                  e.ApplicationMessage.UserProperties[0].Value, 
                  e.ApplicationMessage.UserProperties[1].Value,
                  e.ClientId);

              //return Task.CompletedTask;
            };


            await mqttClient.SubscribeAsync(new MqttTopicFilterBuilder()
                 .WithTopic("$share/ReadApiserviceGroup/watertank")
                 .Build(), ctoken);

     
            await Task.Delay(-1, ctoken);

            Console.WriteLine($"MqttSub session closed {DateTime.UtcNow} utc");
            //}
        }
    }
}
