using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Dbcheck
{
    public class Dbinit
    {
        public  string Connstring { get; }
        private readonly ILogger? logger;
        private readonly bool logENV = false;
        public Dbinit(IConfiguration? _conf, ILogger<Dbinit>? _logger, string? DirectConnectionstring = null)
        {

            logger = _logger;


            try
            {
                if (DirectConnectionstring is null)
                {
                    if (bool.TryParse(_conf["LOGGING_ALLINFO"], out bool logENV)) { }
                    ;

                    Connstring = "Host=" + (
                    _conf["DCOMPOSE_DATABASEHOST"] ??
                    _conf.GetConnectionString("postgwhost") ?? "localhost");

                    Connstring += _conf.GetConnectionString("postgwbody") ?? throw new NotImplementedException("db connection body string missing ; check appsetting.json");
                }
                else
                {
                    Connstring = DirectConnectionstring!;
                }
            }
            catch (Exception ex)
            {
                if (logger is not null)
                    logger!.LogWarning("{}", ex.Message);
            }

        }


        public async Task InitCreationAsync()
        {

            int initretry = 1;
            const int initretryCAP = 3;

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

            retrylogicAsync:
            try
            {


                using var conn = new NpgsqlConnection(Connstring);


                await conn.OpenAsync();

                if (logger is not null)
                    logger.LogInformation("database connection , SUCCESSFUL");

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
                if (logger is not null)
                    logger.LogInformation("db Hypertable tables initialized/checked successfully");



                //var x = await conn.ExecuteAsync(sqltable);
                //var y = await conn.ExecuteAsync(sqlhypertable);

            }
            catch (Exception ex)
            {

                logger.LogWarning("{}", ex.Message);

                if(initretry <= initretryCAP)
                {

                    logger.LogWarning("{}", $"{initretry} retry attempt(s) to connect");
                    await Task.Delay(5000);
                    initretry++;
                    goto retrylogicAsync;
                }

            }

        }


        public void InitCreation()
        {

            int initretry = 1;
            const int initretryCAP = 3;

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

            retrylogic:

            try
            {


                using var conn = new NpgsqlConnection(Connstring);


                conn.Open();

                logger.LogInformation("database connection , SUCCESSFUL");
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

            }
            catch (Exception ex)
            {

                logger.LogWarning("{}", ex.Message);

                if (initretry <= initretryCAP)
                {

                    logger.LogWarning("{}", $"{initretry} retry attempt(s) to connect");
                    Thread.Sleep(5000);
                    initretry++;
                    goto retrylogic;
                }
            }


            //var x = await conn.ExecuteAsync(sqltable);
            //var y = await conn.ExecuteAsync(sqlhypertable);

        }


    }
}
