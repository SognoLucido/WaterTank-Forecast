using Dapper;
using Npgsql;
using Npgsql.Internal;

namespace DataFlow_ReadAPI.Services.DBFetching
{
    public class DbFetch : IDbFetch
    {
        private readonly string Connstring;

        

        public DbFetch(IConfiguration _conf) { Connstring = _conf.GetConnectionString("postgresread")!; }


        public async Task Fetchdata()
        {
          
            const string sqlfetch = @"
                WITH tank_last_time AS (
	                SELECT DISTINCT ON (tank_id)
                    tank_id,
	                time AS last_time ,
                    current_volume
	                FROM watertank
	                ORDER BY tank_id, time DESC
                ),
                date_ranges AS (
                    SELECT 
                        tank_id,
                        last_time,
		                tl.current_volume AS lastvol,
                        COALESCE(
                            (SELECT MIN(time) 
                             FROM public.watertank wt 
                             WHERE wt.tank_id = tl.tank_id AND wt.time >= tl.last_time - INTERVAL '7 days'),
                            last_time - INTERVAL '7 days'
                        ) AS start_time
                    FROM tank_last_time tl
                ),
                consumption_data AS (
                  SELECT 
                        wt.tank_id,
                        wt.time::date AS consumption_day,
                        wt.current_volume,
                        LAG(wt.current_volume) OVER (PARTITION BY wt.tank_id ORDER BY wt.time) AS previous_volume,
                        CASE 
                            WHEN wt.current_volume < LAG(wt.current_volume) OVER (PARTITION BY wt.tank_id ORDER BY wt.time)
                                 THEN LAG(wt.current_volume) OVER (PARTITION BY wt.tank_id ORDER BY wt.time) - wt.current_volume
                            ELSE 0
                        END AS daily_consumption,
                        CASE 
                            WHEN wt.current_volume > LAG(wt.current_volume) OVER (PARTITION BY wt.tank_id ORDER BY wt.time)
                                 THEN 1
                            ELSE 0
                        END AS is_refill
                    FROM public.watertank wt
                    JOIN date_ranges dr
                    ON wt.tank_id = dr.tank_id
                    WHERE wt.time BETWEEN dr.start_time AND dr.last_time
                ),
                summary_data AS (
                    SELECT 
                        tank_id,
                        SUM(daily_consumption) AS total_consumption,
                        COUNT(DISTINCT consumption_day) FILTER (WHERE is_refill = 0) AS non_refill_days
                    FROM consumption_data
                    GROUP BY tank_id
                ),
                consumption_calc as(
                SELECT 	
                    dr.tank_id,
	                dr.lastvol ,
                    dr.start_time AS range_start,
                    dr.last_time AS range_end,
                    sd.total_consumption,
                    sd.non_refill_days,
                    CASE 
                        WHEN sd.non_refill_days > 1 THEN (sd.total_consumption )  / (sd.non_refill_days - 1)
                        ELSE NULL
                    END AS average_daily_consumption
	
                FROM summary_data sd
                JOIN date_ranges dr
                ON sd.tank_id = dr.tank_id
                )
                SELECT tank_id , range_end ,  (g.lastvol / g.average_daily_consumption) as days_remaining   FROM consumption_calc as g";

            using (var Dbconn = new NpgsqlConnection(Connstring))
            {

                var data = Dbconn.Query(
                  sqlfetch)
                  .Select(x => new {
                      x.tank_id,
                      x.range_end,
                      x.days_remaining
                  })
                  .ToList();



            }

        }

    }
}
