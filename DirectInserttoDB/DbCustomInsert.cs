using Dapper;
using DirectInserttoDB.Models;
using Npgsql;

namespace DirectInserttoDB;



public class WaterTank
{
    public DateTime Time { get; set; }
    public Guid Id { get; set; }
    public double Lvl { get; set; }
    public Guid? Clientid { get; set; }
    public string? Zcode { get; set; }
}


internal class DbCustomInsert
{
    const string Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";



    public async Task Start()
    {

        using var Dbconn = new NpgsqlConnection(Connstring);

        const string sqlinsert = @"INSERT INTO watertank (time, tank_id, current_volume, client_id, zone_code)
                                   VALUES (@Time, @Id, @Lvl, @Clientid, @Zcode)";


        Guid clientid = Guid.NewGuid();

        // Create a list of WaterTank objects
        //var tanks = new List<WaterTank>
        //{
        //    new WaterTank { Time = DateTime.UtcNow, Id = Guid.NewGuid(), Lvl = 50.5, Clientid =  clientid, Zcode = "Z1" },
        //    new WaterTank { Time = DateTime.UtcNow, Id = Guid.NewGuid(), Lvl = 42.0, Clientid =  clientid, Zcode = "Z2" },
        //    new WaterTank { Time = DateTime.UtcNow, Id = Guid.NewGuid(), Lvl = 60.0, Clientid = clientid, Zcode = "Z3" }
        //};

        var tanks = new BodyData[]
        {
            new(),
            new(),
            new()
        };
      
          
            int rowsInserted = Dbconn.Execute(sqlinsert, tanks);
            Console.WriteLine($"{rowsInserted} rows inserted.");

        return;


       // string sqlinsert = @"INSERT INTO watertank (time ,tank_id, current_volume )VALUES (@Time , @Id , @Lvl )";

        string sqltest2 = @"INSERT INTO watertank (time ,tank_id, current_volume,client_id,zone_code)VALUES (@Time , @Id , @Lvl,@CLIENTID , @ZCODE )";






        // Guid guid = Guid.NewGuid();

        Guid guid = new("841BA82E-6A6C-43E5-865B-A8D0DAE18D9D");

        DateTime date1 = new DateTime(2025, 01, 01, 12, 00, 00, DateTimeKind.Utc);
        //DateTime date2 = new DateTime(2025, 01, 02, 8, 00, 00, DateTimeKind.Utc);
        //DateTime date3 = new DateTime(2025, 01, 03, 8, 00, 00, DateTimeKind.Utc);
        //DateTime date3_1 = new DateTime(2025, 01, 03, 10, 00, 00, DateTimeKind.Utc);
        //DateTime date3_2 = new DateTime(2025, 01, 03, 12, 00, 00, DateTimeKind.Utc);
        //DateTime date3_3 = new DateTime(2025, 01, 03, 14, 00, 00, DateTimeKind.Utc);
        //DateTime date3_4 = new DateTime(2025, 01, 03, 20, 00, 00, DateTimeKind.Utc);
        //DateTime date4 = new DateTime(2025, 01, 04, 8, 00, 00, DateTimeKind.Utc);
        //DateTime date5 = new DateTime(2025, 01, 05, 8, 00, 00, DateTimeKind.Utc);
        //DateTime date6 = new DateTime(2025, 01, 06, 8, 00, 00, DateTimeKind.Utc);
        //DateTime date7 = new DateTime(2025, 01, 07, 8, 00, 00, DateTimeKind.Utc);
        //DateTime date8 = new DateTime(2025, 01, 08, 8, 00, 00, DateTimeKind.Utc);



        var data = new List<object>()
        {
            // new { Time = date1,Id = guid,Lvl = (double)720 , CLIENTID = new Guid("93CA9333-B607-4E74-B95C-9C2006D07BDB"),ZCODE = "pep" },
            //new { Time = date2,Id = guid,Lvl = (double)714 },
            //new { Time = date3,Id = guid,Lvl = (double)696 },
            //new { Time = date3_1,Id = guid,Lvl = (double)696 },
            // new { Time = date3_2,Id = guid,Lvl = (double)696 },
            //  new { Time = date3_3,Id = guid,Lvl = (double)696 },
            //   new { Time = date3_4,Id = guid,Lvl = (double)691 },

            //new { Time = date4,Id = guid,Lvl = (double)689 },
            //new { Time = date6,Id = guid,Lvl = (double)680 },
            //new { Time = date7,Id = guid,Lvl = (double)678 },
            // new { Time = date8,Id = guid,Lvl = (double)675 }

        };



        //calcul(Items);

        await Dbconn.ExecuteAsync(sqltest2, new { Time = date1, Id = guid, Lvl = (double)720, CLIENTID = new Guid("93CA9333-B607-4E74-B95C-9C2006D07BDB"), ZCODE = "pep" });







    }


}
