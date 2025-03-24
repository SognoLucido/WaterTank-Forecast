using System.Diagnostics;
using Dapper;
using DirectInserttoDB.Models;
using Npgsql;

namespace DirectInserttoDB;

internal class DBBulkinsert
{


    const string Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";



    public async Task Start(int itemX)
    {
        const int days = 30;
       // const int TOTAL_ITEMS = 10000;
        const string sqlinsert = @"INSERT INTO watertank (time ,tank_id, current_volume,client_id,zone_code,total_capacity)VALUES (@Time , @Id , @Lvl,@Clientid, @Zcode,@Totcapacity )";

        using var Dbconn = new NpgsqlConnection(Connstring);
        //BodyData[] Items = new BodyData[TOTAL_ITEMS];
        BodyData[] Items = new BodyData[itemX];

        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = new();
        }


        var Stime = Stopwatch.GetTimestamp();


        for (int i = 0; i < days; i++)
        {

            foreach (var data in Items)
            {
                if (i > 0)
                {
                    data.Time = data.Time.AddDays(1);
                    data.Scaledown();
                }


            }


          await Dbconn.ExecuteAsync(sqlinsert, Items);

            Console.WriteLine($"{i+1}/{days} + batch inserted");


            
        }


        var timerange = Stopwatch.GetElapsedTime(Stime, Stopwatch.GetTimestamp());

        Console.WriteLine(timerange);


    }
}
