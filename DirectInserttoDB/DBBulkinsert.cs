using System.Diagnostics;
using Dapper;
using DirectInserttoDB.Models;
using Npgsql;

namespace DirectInserttoDB;

internal class DBBulkinsert
{


    const string Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";



    public async Task Start()
    {

        using var Dbconn = new NpgsqlConnection(Connstring);
        BodyData[] Items = new BodyData[10000];


        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = new();
        }



        const string sqlinsert = @"INSERT INTO watertank (time ,tank_id, current_volume )VALUES (@Time , @Id , @Lvl )";

        const int days = 31;


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


            //calcul(Items);

          await Dbconn.ExecuteAsync(sqlinsert, Items);

            Console.WriteLine($"{i+1}/{days} + batch inserted");


            
        }


        var timerange = Stopwatch.GetElapsedTime(Stime, Stopwatch.GetTimestamp());

        Console.WriteLine(timerange);


    }
}
