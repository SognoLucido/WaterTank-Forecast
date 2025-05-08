using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Dbcheck
{
    public class Dbinit
    {
        public string Connstring { get; }

        public Dbinit(IConfiguration _conf)
        {

            Connstring = "Host=" + (
                _conf["DCOMPOSE_DATABASEHOST"] ??
                _conf.GetConnectionString("postgwhost") ?? "localhost");

            Connstring += _conf.GetConnectionString("postgwbody") ?? throw new NotImplementedException("db connection body string missing ; check appsetting.json");
        }


        public async Task InitCreationAsync()
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


            await conn.OpenAsync();

            //const string checkDbQuery = "SELECT 1 FROM pg_database WHERE datname = 'WaterTank'";
            //var result = await conn.ExecuteScalarAsync(checkDbQuery);

            using var cmd = new NpgsqlCommand(sqltable, conn);
            using var cmd1 = new NpgsqlCommand(sqlhypertable, conn);

            //try
            //{
                await cmd.ExecuteNonQueryAsync();
                await cmd1.ExecuteNonQueryAsync();
            //}
            //catch (Exception ex)
            //{

            //}
            



            //var x = await conn.ExecuteAsync(sqltable);
            //var y = await conn.ExecuteAsync(sqlhypertable);

        }


        public void InitCreation()
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


            conn.Open();

            //const string checkDbQuery = "SELECT 1 FROM pg_database WHERE datname = 'WaterTank'";
            //var result = await conn.ExecuteScalarAsync(checkDbQuery);

            using var cmd = new NpgsqlCommand(sqltable, conn);
            using var cmd1 = new NpgsqlCommand(sqlhypertable, conn);

            //try
            //{
             cmd.ExecuteNonQuery();
             cmd1.ExecuteNonQuery();
            //}
            //catch (Exception ex)
            //{

            //}




            //var x = await conn.ExecuteAsync(sqltable);
            //var y = await conn.ExecuteAsync(sqlhypertable);

        }


    }
}
