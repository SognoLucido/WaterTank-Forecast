

using MQTTnet.Client;
using MQTTnet;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;
using System;
using WaterTankMock_MQTT.Models;
using System.Linq;

namespace WaterTankMock_MQTT.Services;

public class MqttInit
{
    private readonly List<IMqttClient> clients = [];
    private readonly MqttFactory factory = new();
    public event EventHandler<bool>? ConnectionStatus;

    readonly Sharedata Sharedata;
  

    public MqttInit(Sharedata sharedata)
    {
        Sharedata = sharedata;
    }

    public async Task Checkconnection(string ip,int port,CancellationToken ctoken)
    {
        using var mqttClient = factory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, port)
            .WithCleanSession(true)
            .WithClientId("ConnectionCheck")
            .WithKeepAlivePeriod(TimeSpan.FromSeconds(60))
            .Build();
        try
        {
            
           

            await mqttClient.ConnectAsync(mqttClientOptions, CancellationToken.None);

            var applicationMessage = new MqttApplicationMessageBuilder()
              .WithTopic("test")
              .WithPayload("testconnection")
              .Build();

            var test = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

             if (mqttClient.IsConnected) ConnectionStatus?.Invoke(this, true);

            while (!mqttClient.IsConnected || !ctoken.IsCancellationRequested)
            {

                await Task.Delay(5000,ctoken);
            }


        }
        catch (Exception)
        {
            ConnectionStatus?.Invoke(this, false);

        }

        ConnectionStatus?.Invoke(this, false);


    }
}
