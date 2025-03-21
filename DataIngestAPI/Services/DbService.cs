
using System.Text.Json;
using Dapper;
using DataIngestAPI.Models;
using Microsoft.Extensions.Configuration;
using Npgsql;

[module:DapperAot]
namespace DataIngestAPI.Services
{
    public class DbService : IDbService
    {
        //private string Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";
        private readonly string Connstring;
        public DbService(IConfiguration conf) { Connstring = conf.GetConnectionString("postgreswrite")!; }
        public async Task InitCreation()
        {
          
            const string sqltable = @"
            CREATE TABLE IF NOT EXISTS watertank (
            time TIMESTAMPTZ NOT NULL,                                   
            tank_id UUID NOT NULL,
            current_volume DOUBLE PRECISION NOT NULL,
            client_id UUID,
            zone_code VARCHAR(10),
            total_capacity DOUBLE PRECISION
                );";

            const string sqlhypertable = @"SELECT create_hypertable('WaterTank', by_range('time') , if_not_exists => TRUE);";


            using var conn = new NpgsqlConnection(Connstring);

            
            //await conn.OpenAsync();

            //const string checkDbQuery = "SELECT 1 FROM pg_database WHERE datname = 'WaterTank'";
            //var result = await conn.ExecuteScalarAsync(checkDbQuery);


            var x =  await conn.ExecuteAsync(sqltable);
            var y = await conn.ExecuteAsync(sqlhypertable);

        }

        public async Task Insertdata(string data)
        {


            //try
            //{

                var TankItemclass = JsonSerializer.Deserialize<WaterTankItem>(data, WaterTankItemSGjsonConverter.Default.WaterTankItem);

            //}
            //catch(Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}

            using var conn = new NpgsqlConnection(Connstring);


            //Random rnd = new Random();

            //for (var i = 0; i < 10; i++)
            //{
            //    var waterTankData = new
            //    {
            //        Time = DateTime.UtcNow, // Ensure this is UTC time
            //        TankId = Guid.NewGuid(),
            //        CurrentVolume = rnd.Next(100),
            //        TotalCapacity = 1000

            //    };


            const string insertQuery = @"
            INSERT INTO WaterTank (time, tank_id, current_volume, total_capacity)
            VALUES (@timestamp, @tank_id, @current_volume, @total_capacity)";



            var result = await conn.ExecuteAsync(insertQuery, TankItemclass);

            //Console.WriteLine(TankItemclass.tank_id);
           // await conn.CloseAsync();
            // Console.WriteLine($"ID: {TankItemclass.tank_id} , CurrentCap: {TankItemclass.current_volume} ->parsed & successful inserted in db" );
           
        }

        //optional
        public async Task Cleantable()
        {
            const string sqldel = @"DROP TABLE IF EXISTS WaterTank";

            using var conn = new NpgsqlConnection(Connstring);

            var x = await conn.ExecuteAsync(sqldel);

            Console.WriteLine("table cleaned");
        }


    }
}
