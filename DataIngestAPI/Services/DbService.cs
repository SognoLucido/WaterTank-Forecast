
using System.Text.Json;
using Dapper;
using DataIngestAPI.Models;
using Dbcheck;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

[module:DapperAot]
namespace DataIngestAPI.Services
{
    public class DbService(Dbinit _dbinit,ILogger<DbService> _logger) : IDbService
    {
        private readonly ILogger<DbService> logger = _logger;
        private readonly Dbinit dbinit = _dbinit;
        private string Connstring = string.Empty;
        private bool InfoLogEnable;
        

        public async Task InitCreation(bool _enableinfoLog)
        {
            InfoLogEnable = _enableinfoLog;
            await dbinit.InitCreationAsync();

            Connstring = dbinit.Connstring;
        }


        /// <summary>
        ///  the _idTankitem and _idMqttclient are the same because the mqttMocktoolDesktopApp, used for simulating the sensors, assigns the tankgenerateid as the clientmqttid as well
        /// </summary>
        /// <param name="data">body data sent by the "embedded" device (capacity, timestamp, etc.)</param>
        /// <param name="_idTankitem">Tank UUID </param>
        /// <param name="_idMqttclient"> Tank item MQTT client UUID used to connect with the MQTTbroker</param>
        /// <param name="_thisAPPsubmqttID">bridge app (this app) subscription ID used by this app to connect to the MQTT broker</param>
        public async Task Insertdata(string data, string _idTankitem, string _idMqttclient, string _thisAPPsubmqttID)
        {

           
           

            try
            {

              //  throw new NotImplementedException();

                var TankItemclass = JsonSerializer.Deserialize<WaterTankItem>(data, WaterTankItemSGjsonConverter.Default.WaterTankItem);


                using var conn = new NpgsqlConnection(Connstring);


                const string insertQuery = @"
                INSERT INTO WaterTank (time, tank_id, current_volume,client_id,zone_code, total_capacity)
                VALUES (@timestamp, @tank_id, @current_volume,@client_id,@zone_code, @total_capacity)";



                var result = await conn.ExecuteAsync(insertQuery, TankItemclass);

                if(InfoLogEnable) logger.LogInformation ("TankitemId: {TankitemId} --- MqttClientId: {MqttClientId} --- SubId: {SubId}",  _idTankitem, _idMqttclient, _thisAPPsubmqttID);

            }
            catch (Exception ex) 
            {
               
                logger.LogWarning("{Timestamp:yyyy-MM-dd HH:mm:ss} UTC --- TankitemId: {TankitemId} --- MqttClientId: {MqttClientId} --- SubId: {SubId} --- Exception: {ExceptionMessage}",DateTime.UtcNow, _idTankitem, _idMqttclient, _thisAPPsubmqttID, ex.Message);
            }
           
        }

      


    }
}
