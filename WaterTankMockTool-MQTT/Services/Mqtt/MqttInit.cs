

using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Formatter;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services.Mqtt.Models;


namespace WaterTankMock_MQTT.Services.Mqtt;

public class MqttInit
{
    private readonly List<IMqttClient> clients = [];
    private MqttClientFactory? factory; // to fix this create new one on connection not on class init

    public event EventHandler<bool>? ConnectionStatus;
    private MqttClientOptions? options;
    private string? ip;
    private int? port;
    private List<MqttBodyJsonModel>? rawdataclients;
    private CancellationToken Onlinetoken;
    //int[] Htriggers = [3,5,7,9,11,13,15,17,19,21];


    readonly Sharedata Sharedata;

    public MqttInit(Sharedata sharedata)
    {
        Sharedata = sharedata;
    }

    public async Task Checkconnection(string ip, int port, CancellationToken ctoken)
    {
        Onlinetoken = ctoken;

        factory = new();

        using var mqttClient = factory.CreateMqttClient();
        var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithProtocolVersion(MqttProtocolVersion.V500)
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

            while (!ctoken.IsCancellationRequested && mqttClient.IsConnected)
            {

                await Task.Delay(5000, ctoken);
            }


        }
        catch (Exception)
        {
            ConnectionStatus?.Invoke(this, false);

        }

        ConnectionStatus?.Invoke(this, false);
        await Cleanup();

    }

    private async Task Cleanup()
    {

        foreach (var mqttClient in clients)
        {
            mqttClient.Dispose();
        }

        clients.Clear();
        factory = null;
        ResetTriggers();

        ip = null;
        port = null;
        rawdataclients = null;
    }



    public async Task Startsim()
    {
        Sharedata.Mqttbusy = true;
        Sharedata.Daycount = 0;
        Sharedata.ProgressBar = 0;
        var DateBackup = Sharedata.StartTestDate;

        await ClientConnector();


        if (Sharedata.Seeddata)
            await Seeddata();


        await Start();

        Sharedata.Mqttbusy = false;
    }



    private async Task Start() //buggy to fix
    {

        Random rng = new();
        var options = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        Sharedata.ProgressBar = 0;
        Sharedata.ProgressMessage = "Sending...";

        var applicationMessage = new MqttApplicationMessageBuilder()
        .WithTopic(Sharedata.MqttTopic)
        .WithUserProperty("tankid", "")
        .WithUserProperty("mqttid", "")
        //.WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
        .Build();


        //this datetime used to the logic in the loop (mqqt timestamp body ) 
        var Logicdate = new DateTime(
            Sharedata.StartTestDate.Year,
            Sharedata.StartTestDate.Month,
            Sharedata.StartTestDate.Day,
            3,
            Sharedata.StartTestDate.Minute,
            Sharedata.StartTestDate.Second, kind: DateTimeKind.Utc
            );


        int triggerStateProgressbar = 0;

        // Sharedata.StartTestDate  // Display DateTime; this shows the progress 




        for (; Sharedata.Daycount < Sharedata.Toxdays; Sharedata.Daycount++)
        {


            //  var testdatepint = Sharedata.StartTestDate;
            //await Task.Delay(2000);

            //   Sharedata.Simtriggers[0] = true;

            await Task.Delay(1000);

            for (int i = 0, t = 0; ; ++i)  // clients i  ; triggers t
            {

                if (i >= Sharedata.Items.Count)
                {
                    triggerStateProgressbar++;
                    Sharedata.Simtriggers[t] = true;
                    t++;
                    i = 0;
                    Sharedata.StartTestDate = Logicdate;
                    if (t >= Sharedata.Simtriggers.Count)
                    {

                        Logicdate = Logicdate.AddHours(6);
                        break;
                    }
                    else
                    {

                        Logicdate = Logicdate.AddHours(2);
                    }
                    //if (t >= Sharedata.Simtriggers.Count)
                    //{
                    //    // Sharedata.Simtriggers[t - 1] = true;
                    //    Sharedata.StartTestDate = Sharedata.StartTestDate.AddHours(6);
                    //    break;
                    //}
                    //else
                    //{

                    //    //Sharedata.Simtriggers[t-1] = true;
                    //    /* if (t > 1)*/
                    //    Sharedata.StartTestDate = Sharedata.StartTestDate.AddHours(2);
                    //}

                    await Task.Delay(200);
                }

                if (Sharedata.Items[i].Triggers[t].Active)
                {
                    if (Onlinetoken.IsCancellationRequested) return;

                    var resourcescale = rng.Next(Sharedata.Items[i].Triggers[t].Rangemin, Sharedata.Items[i].Triggers[t].Rangemax);
                    Sharedata.Items[i].CurrentLevel = (Sharedata.Items[i].CurrentLevel - resourcescale) < 0 ? 0 : Sharedata.Items[i].CurrentLevel - resourcescale;


                    //Console.WriteLine(Sharedata.StartTestDate);

                    var start = Sharedata.Items[i].Triggers[t].Active;
                    var test = Sharedata.StartTestDate;

                    applicationMessage.PayloadSegment = JsonSerializer.SerializeToUtf8Bytes(
                         new MqttBodyJsonModel
                         {
                             tank_id = Sharedata.Items[i].Id,
                             timestamp = Logicdate, // OLDSharedata.StartTestDate, // trigger 3:am
                             current_volume = Sharedata.Items[i].CurrentLevel,
                             total_capacity = Sharedata.Capacity ? Sharedata.Items[i].Capacity : null,
                             client_id = Sharedata.ClientidEnable ? Sharedata.Clientguid : null,
                             zone_code = Sharedata.ZonecodeEnable ? Sharedata.ZonecodeID : null,


                         }, options
                   );

                  

                    if (!Onlinetoken.IsCancellationRequested)
                    {

                        applicationMessage.UserProperties[0] = new(applicationMessage.UserProperties[0].Name, Sharedata.Items[i].Id.ToString());
                        applicationMessage.UserProperties[1] = new(applicationMessage.UserProperties[1].Name, clients[i].Options.ClientId);
                        await clients[i].PublishAsync(applicationMessage, Onlinetoken);
                    }
                    else return;

                }


                //  Sharedata.ProgressBar = (int)(((t+1)*(Sharedata.Daycount +1)) * (100.0 / Sharedata.Simtriggers.Count  * Sharedata.Toxdays));

                Sharedata.ProgressBar = (int)(100 * (triggerStateProgressbar) / ((10 * Sharedata.Toxdays) - 1));



            }

            await Task.Delay(200);

            ResetTriggers();

            Sharedata.StartTestDate = Sharedata.StartTestDate.AddHours(3);

        }

        Sharedata.ProgressMessage = "Done";

    }



    private void ResetTriggers()
    {

        for (int z = 0; z < Sharedata.Simtriggers.Count; z++)
        {

            Sharedata.Simtriggers[z] = false;
        }
    }

    private async Task ClientConnector()
    {

        options = new MqttClientOptionsBuilder()
            .WithProtocolVersion(MqttProtocolVersion.V500)
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

            await clients[i].ConnectAsync(options, Onlinetoken);


            Sharedata.ProgressBar = (int)((i + 1) * (100.0 / Sharedata.Items.Count));
            await Task.Delay(500);
        }


        Sharedata.ProgressMessage = $"All {Sharedata.Items.Count} clients connected";

        await Task.Delay(2000);



        //options.ClientId = "hewae";

    }


    private async Task Seeddata()
    {

        Sharedata.ProgressMessage = "Seeding...";

        Sharedata.ProgressBar = 0;


        await Task.Delay(1000);
        var applicationMessage = new MqttApplicationMessageBuilder()
          .WithTopic(Sharedata.MqttTopic)
          .Build();


        rawdataclients = [];



        foreach (var item in Sharedata.Items)
        {
            rawdataclients.Add
                (
                    new MqttBodyJsonModel
                    {
                        tank_id = item.Id,
                        timestamp = Sharedata.StartTestDate,
                        current_volume = item.CurrentLevel,
                        total_capacity = item.Capacity
                        //capacity = new()
                        //{
                        //    current_volume = item.CurrentLevel,
                        //    total_capacity = item.Capacity
                        //}

                    }
                );
        }


        for (int i = 0; i < clients.Count; i++)
        {

            applicationMessage.PayloadSegment = JsonSerializer.SerializeToUtf8Bytes(rawdataclients[i]);
            await clients[i].PublishAsync(applicationMessage, Onlinetoken);
            Sharedata.ProgressBar = (int)((i + 1) * (100.0 / Sharedata.Items.Count));
        }


    }





}
