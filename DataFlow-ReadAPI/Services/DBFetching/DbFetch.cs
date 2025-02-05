using Dapper;
using DataFlow_ReadAPI.Models;
using Npgsql;
using Npgsql.Internal;

namespace DataFlow_ReadAPI.Services.DBFetching
{
    public class DbFetch : IDbFetch
    {
        private readonly string Connstring;

        

        public DbFetch(IConfiguration _conf) { Connstring = _conf.GetConnectionString("postgresread")!; }


        public async Task<IEnumerable<DBreturnData>> Fetchdata(Guid[]? tank_ids ,  int Rangedays )
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
                        last_time::date AS last_date,
		                tl.current_volume AS last_volume,
                        COALESCE(
                            (SELECT MIN(time)::date 
                             FROM public.watertank wt 
                             WHERE wt.tank_id = tl.tank_id AND wt.time >= tl.last_time - INTERVAL '7days'), 
                            (last_time - INTERVAL '7days')::date
                        ) AS start_date
                    FROM tank_last_time tl
                ),
                all_dates AS (
                    SELECT 
                        dr.tank_id, 
                        generate_series(dr.start_date, dr.last_date, '1 day'::interval)::date AS consumption_day
                    FROM date_ranges dr
                ),
                consumption_data AS (
                    SELECT 
                        ad.tank_id,
                        ad.consumption_day,
                        wt.current_volume
                    FROM all_dates ad
                    LEFT JOIN public.watertank wt 
                        ON ad.tank_id = wt.tank_id AND ad.consumption_day = wt.time::date
                ),
                filled_data AS (
                    SELECT 
                        tank_id,
                        consumption_day,
                        COALESCE(
                            current_volume, 
                            LAG(current_volume) OVER (PARTITION BY tank_id ORDER BY consumption_day) 
                        ) AS filled_volume
                    FROM consumption_data
                ),
                final_consumption AS (
                    SELECT 
                        tank_id,
                        consumption_day,
                        filled_volume AS current_volume,
                        LAG(filled_volume) OVER (PARTITION BY tank_id ORDER BY consumption_day) AS previous_volume,
                        CASE 
                            WHEN filled_volume < LAG(filled_volume) OVER (PARTITION BY tank_id ORDER BY consumption_day)
                            THEN LAG(filled_volume) OVER (PARTITION BY tank_id ORDER BY consumption_day) - filled_volume
                            ELSE 0
                        END AS daily_consumption,
                        CASE 
                            WHEN filled_volume > LAG(filled_volume) OVER (PARTITION BY tank_id ORDER BY consumption_day)
                            THEN 1
                            ELSE 0
                        END AS is_refill
                    FROM filled_data
                ),
                summary_data AS(
                    SELECT 
                        tank_id,
                        SUM(daily_consumption) AS total_consumption,
                        (COUNT(DISTINCT consumption_day) FILTER (WHERE is_refill = 0) -1) AS non_refill_days
                    FROM final_consumption
                    GROUP BY tank_id
                ),
                recap_data AS(
                SELECT 
                    dr.tank_id,
                    dr.last_date AS range_end,
	                dr.last_volume,
                    CASE 
                        WHEN sd.non_refill_days > 1 
                        THEN sd.total_consumption / sd.non_refill_days 
                        ELSE NULL
                    END AS average_daily_consumption
                FROM summary_data sd
                JOIN date_ranges dr ON sd.tank_id = dr.tank_id
                )
                SELECT g.tank_id, g.range_end + INTERVAL '1 day' * (g.last_volume/ g.average_daily_consumption )::INTEGER as empty_at_day 
                FROM recap_data as g";

            // OLD SELECT g.tank_id, g.range_end ,  (g.last_volume / g.average_daily_consumption) as days_remaining   FROM recap_data as g"

            using (var Dbconn = new NpgsqlConnection(Connstring))
            {


               var x = await Dbconn.QueryAsync<DBreturnData>(sqlfetch);



                return x;

                //var data = Dbconn.Query(
                //  sqlfetch)
                //  .Select(x => new {
                //      x.tank_id,
                //      x.range_end,
                //      x.days_remaining
                //  })
                //  .ToList();



            }

        }

    }
}
