using System.Data;
using System.Data.Common;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;
using Dapper;
using DataFlow_ReadAPI.Models;
using Microsoft.AspNetCore.Components.Forms;
using Npgsql;
using Npgsql.Internal;

namespace DataFlow_ReadAPI.Services.DBFetching
{
    public class DbFetch : IDbFetch
    {
        private readonly string Connstring;
     
        public DbFetch(IConfiguration _conf) { Connstring = _conf.GetConnectionString("postgresread")!; }

        
        public async Task<DBreturnDataDto?> Forecast(Guid[]? tank_ids, int Rangedays, Guid? clientid, string? zcode)
        {
         

            var sb = new StringBuilder();
            var param = new DynamicParameters();

            param.Add("DAYS", Rangedays, DbType.Int32);

            if(tank_ids is not null && tank_ids.Length > 0)
            {
                sb.Append("WHERE tank_id = ANY(@TankIds) ");
                param.Add("TankIds", tank_ids);


                if(clientid is not null)
                {
                    sb.Append("AND client_id = @CLIENTID ");
                    param.Add("CLIENTID", clientid, DbType.Guid );
                }
                if(zcode is not null)
                {
                    sb.Append("AND zone_code = @ZONE ");
                    param.Add("ZONE", zcode, DbType.String);
                }


            }
            else
            {
                sb.Append("WHERE ");
                bool whereEnable = false;

                if (clientid is not null)
                {
                    sb.Append("client_id = @CLIENTID ");
                    param.Add("CLIENTID", clientid, DbType.Guid);
                    whereEnable = true;
                }
                if(zcode is not null)
                {
                    if(whereEnable)sb.Append("AND zone_code = @ZONE ");
                    else sb.Append("zone_code = @ZONE ");
                    param.Add("ZONE", zcode, DbType.String);
                }

            }


            // WHERE tank_id = ANY(@TankIds)


            string sqlfetch = $@"
                                 WITH tank_last_time AS (
                       SELECT DISTINCT ON (tank_id)
                       tank_id,
                       time AS last_time ,
                       current_volume
                       FROM watertank
                       {sb}
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
                             WHERE wt.tank_id = tl.tank_id AND wt.time >= tl.last_time - (@DAYS * INTERVAL '1 day')), 
                            (last_time - (@DAYS * INTERVAL '1 day'))::date
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


               var x = await Dbconn.QueryAsync<DbforecastreturnData>(sqlfetch, param);

                if (x.Any())
                {
                    return new DBreturnDataDto
                    {
                        RangeDays = Rangedays,
                        Clientid = clientid,
                        Zonecode = zcode,
                        Data = x
                    };
                }
                else return null;


            }

        }
                
      
        public async Task<IEnumerable<DbInfoItemwithDATEtime>?> GetinfoItem(Guid[] Ids, bool clientid, bool zcode, bool totcap ,DateTime min , DateTime max)
        {

            if(max.Hour == 00 && max.Minute == 00 && max.Second == 00)
                max = max.AddDays(1);


            StringBuilder sb = new();

            sb.Append(',');

            if (clientid) sb.Append("'client_id', client_id,");
            if (zcode) sb.Append("'zone_code', zone_code,");
            if (totcap) sb.Append("'total_capacity', total_capacity,");

            //'client_id', client_id,
            // 'zone_code', zone_code,
            //  'total_capacity', total_capacity
            //{sb}
            sb.Length--;

            using (var Dbconn = new NpgsqlConnection(Connstring))
            {
                string sql = $@" SELECT 
                    tank_id,  
                    jsonb_agg(
                        jsonb_build_object(
                            'time', time,
                            'current_volume', current_volume
                             {sb}
                        ) ORDER BY time ASC     
                    )  AS Jsondata
                FROM watertank
                WHERE (time >= @StartDate AND time <= @EndDate) 
                 AND tank_id = ANY(@TankIds)
                GROUP BY tank_id;";


                var Rawdbdata = await Dbconn.QueryAsync<DbjsonparseDTO>(sql,new { StartDate = min , EndDate = max , TankIds = Ids });


                var options = new JsonSerializerOptions { Converters = { new UtcDateTimeConverter() } };


                var Dbmapped = Rawdbdata.Select(r => new DbInfoItemwithDATEtime
                {
                    tank_id = r.tank_id,
                    data = JsonSerializer.Deserialize<List<DateTimerange>>(r.Jsondata,options)
                }).ToList();



                return Dbmapped.Count == 0 ?  null : Dbmapped;

            }

        }

        public async Task<IEnumerable<DbInfoItem>?> GetinfoItem(Guid[] Ids ,bool clientid, bool zcode, bool totcap)
        {

            StringBuilder sb = new();

            sb.Append(',');

            if (clientid) sb.Append(Watertank.client_id.ToString() + ',');
            if (zcode) sb.Append(Watertank.zone_code.ToString() + ',');
            if (totcap) sb.Append(Watertank.total_capacity.ToString() + ',');


            if (sb.Length > 0) sb.Length--;
         
            



            using var Dbconn = new NpgsqlConnection(Connstring);



            var dbdata = await Dbconn.QueryAsync<DbInfoItem>(
                "SELECT DISTINCT ON(tank_id) time,tank_id,current_volume " +
                $"{sb}" +
                " FROM watertank" +
                " WHERE tank_id = ANY(@TANKID)" +
                " ORDER BY tank_id, time DESC"
                , new { TANKID = Ids });



            if (!dbdata.Any()) return null;

            //foreach(var item in dbdata)
            //{
            //    if (clientid)
            //        if (item.client_id is null) item.Client_id_info = "Client ID unavailable/not set for this record";

            //    if (zcode)
            //        if (item.zone_code is null) item.Zone_code_info = "Zone code unavailable/not set for this record";

            //    if (totcap)
            //        if (item.total_capacity is null) item.Total_capacity_info = "Total capacity unavailable/not set for this record ";
            
            //}

            return dbdata;


        }

      
    }
}
