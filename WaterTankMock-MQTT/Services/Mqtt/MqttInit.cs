

using MQTTnet.Client;
using MQTTnet;
using System.Collections.Generic;

using System.Threading.Tasks;
using System.Threading;
using System;
using WaterTankMock_MQTT.Models;
using MQTTnet.Server;


namespace WaterTankMock_MQTT.Services.Mqtt;

public class MqttInit
{
    private readonly List<IMqttClient> clients = [];
    private readonly MqttFactory factory = new();
    public event EventHandler<bool>? ConnectionStatus;
    private MqttClientOptions? options;
    private string? ip;
    private int? port;


    readonly Sharedata Sharedata;

    public MqttInit(Sharedata sharedata)
    {
        Sharedata = sharedata;
    }

    public async Task Checkconnection(string ip, int port, CancellationToken ctoken)
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

            this.ip = ip;
            this.port = port;


            var applicationMessage = new MqttApplicationMessageBuilder()
              .WithTopic("test")
              .WithPayload("testconnection")
              .Build();

            var test = await mqttClient.PublishAsync(applicationMessage, CancellationToken.None);

            if (mqttClient.IsConnected) ConnectionStatus?.Invoke(this, true);

            while (mqttClient.IsConnected && !ctoken.IsCancellationRequested)
            {

                await Task.Delay(5000, ctoken);
            }


        }
        catch (Exception)
        {
            ConnectionStatus?.Invoke(this, false);

        }

        ConnectionStatus?.Invoke(this, false);


    }

    public async Task Startsim()
    {

        Sharedata.Daycount = 0;
        Sharedata.ProgressBar = 0;
        var DateBackup = Sharedata.StartTestDate;

        await ClientConnector();


        if (Sharedata.Seeddata)
            await Seeddata();

        //for (; Sharedata.Daycount < Sharedata.Toxdays; Sharedata.Daycount++)
        //{


        //    Sharedata.ProgressMessage = "Seeding...";


        //    //await Task.Delay(2000);


        //    const int totalDelay = 2000; 
        //    const int interval = 100;   
        //    int steps = totalDelay / interval; 

        //    for (int i = 0; i < steps; i++)
        //    {
        //        Sharedata.ProgressBar = (int)((i + 1) * (100.0 / steps)); 
        //        await Task.Delay(interval); 
        //    }


        //    Sharedata.ProgressBar = 0;
        //    Sharedata.ProgressMessage = "Sending msgs";




        //    for (int i = 0; i < Sharedata.Simtriggers.Count; i++)
        //    {

        //        Sharedata.Simtriggers[i] = true;
        //        Sharedata.ProgressBar = (int)((i + 1) * (100.0 / steps));
        //        await Task.Delay(1000);
        //    }

        //    for (int i = 0; i < Sharedata.Simtriggers.Count; i++)
        //    {
        //        Sharedata.Simtriggers[i] = false;
        //    }

        //}

    }


    private async Task ClientConnector()
    {

        options = new MqttClientOptionsBuilder()
            .WithTcpServer(ip, port)
            .WithCleanSession(true)
            .WithClientId("")     
            .WithKeepAlivePeriod(TimeSpan.FromMinutes(5))
            .Build();


        Sharedata.ProgressMessage = "Clients connecting...";

        for (int i = 0; i < Sharedata.Items.Count; i++)
        {

           

            clients.Add(factory.CreateMqttClient());

            options.ClientId = Sharedata.Items[i].Id.ToString();
            
            await clients[i].ConnectAsync(options, CancellationToken.None);

            Sharedata.ProgressBar = (int)((i + 1) * (100.0 / Sharedata.Items.Count));
            await Task.Delay(500);
        }


        Sharedata.ProgressMessage = "All clients connected";

        await Task.Delay(1000);



        //options.ClientId = "hewae";

    }


    private async Task Seeddata()
    {

        Sharedata.ProgressMessage = "Seeding...";

        Sharedata.ProgressBar = 0;

        var applicationMessage = new MqttApplicationMessageBuilder()
          .WithTopic(Sharedata.MqttTopic)
          .WithPayload("")
          .Build();



        for (int i = 0; i < clients.Count; i++) 
        {



        }

    }


}
