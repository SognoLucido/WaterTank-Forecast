using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using MQTTnet.Formatter;
using MQTTnet;
using Microsoft.Extensions.Logging;

namespace DataIngestAPI.Services
{
    internal class MqttReaderBG : BackgroundService
    {
        private readonly IConfiguration conf;
        private readonly ILogger logger;
        private readonly DbService dbcall ;
        private string MqttServerIP ;
        private (byte,byte) trylog { get; set; } = (0,3); // 0 start , up to 3 tries // hardcoded

        public MqttReaderBG(DbService _db, IConfiguration _conf, ILogger<MqttReaderBG> _logger)
        {
            conf = _conf;
            logger = _logger;
            dbcall = _db;
            MqttServerIP = conf["DCOMPOSE_MQTTBROKERHOST"] ?? conf.GetConnectionString("mqtt") ?? "localhost";


        }



        protected override async Task ExecuteAsync(CancellationToken ctoken)
        {

            //while (!stoppingToken.IsCancellationRequested)
            //{
            if (bool.TryParse(conf["LOGGING_ALLINFO"], out bool logENV)) { };

            await dbcall.InitCreation(logENV);

            var mqttFactory = new MqttClientFactory();

                using var mqttClient = mqttFactory.CreateMqttClient();

                var mqttClientOptions = new MqttClientOptionsBuilder()
                .WithTcpServer(MqttServerIP)
                .WithProtocolVersion(MqttProtocolVersion.V500)
                .WithKeepAlivePeriod(TimeSpan.FromMinutes(5))
                .Build();

            try
            {
                await mqttClient.ConnectAsync(mqttClientOptions, ctoken);
            }
            catch (Exception ex)
            {
                logger.LogWarning("{}",ex.Message);
                logger.LogInformation("Retring to connect");
            }
            
            
            mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var test = e.ApplicationMessage.UserProperties;


            // debug info  
            // Console.WriteLine($"Topic : {e.ApplicationMessage.Topic} and Message : {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}"); 

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
