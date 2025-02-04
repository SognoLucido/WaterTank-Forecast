using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DirectInserttoDB.Models;
using Npgsql;

namespace DirectInserttoDB;

internal class DbCustomInsert
{
    const string Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";



    public async Task Start()
    {

        using var Dbconn = new NpgsqlConnection(Connstring);
      



        string sqlinsert = @"INSERT INTO watertank (time ,tank_id, current_volume )VALUES (@Time , @Id , @Lvl )";


        Guid guid = Guid.NewGuid();



        DateTime date1 = new DateTime(2025, 01, 01, 8, 00, 00, DateTimeKind.Utc);
        DateTime date2 = new DateTime(2025, 01, 02, 8, 00, 00, DateTimeKind.Utc);
        DateTime date3 = new DateTime(2025, 01, 03, 8, 00, 00, DateTimeKind.Utc);
        DateTime date3_1 = new DateTime(2025, 01, 03, 10, 00, 00, DateTimeKind.Utc);
        DateTime date3_2 = new DateTime(2025, 01, 03, 12, 00, 00, DateTimeKind.Utc);
        DateTime date3_3 = new DateTime(2025, 01, 03, 14, 00, 00, DateTimeKind.Utc);
        DateTime date3_4 = new DateTime(2025, 01, 03, 20, 00, 00, DateTimeKind.Utc);
        DateTime date4 = new DateTime(2025, 01, 04, 8, 00, 00, DateTimeKind.Utc);
        DateTime date5 = new DateTime(2025, 01, 05, 8, 00, 00, DateTimeKind.Utc);
        DateTime date6 = new DateTime(2025, 01, 06, 8, 00, 00, DateTimeKind.Utc);
        DateTime date7 = new DateTime(2025, 01, 07, 8, 00, 00, DateTimeKind.Utc);
        DateTime date8 = new DateTime(2025, 01, 08, 8, 00, 00, DateTimeKind.Utc);

        

        var data = new List<object>() {
          new { Time = date1,Id = guid,Lvl = (double)720 },
          new { Time = date2,Id = guid,Lvl = (double)714 },
          new { Time = date3,Id = guid,Lvl = (double)696 },
          new { Time = date3_1,Id = guid,Lvl = (double)696 },
           new { Time = date3_2,Id = guid,Lvl = (double)696 },
            new { Time = date3_3,Id = guid,Lvl = (double)696 },
             new { Time = date3_4,Id = guid,Lvl = (double)691 },

          new { Time = date4,Id = guid,Lvl = (double)689 },
          new { Time = date6,Id = guid,Lvl = (double)680 },
          new { Time = date7,Id = guid,Lvl = (double)678 },
           new { Time = date8,Id = guid,Lvl = (double)675 }

        };



        //calcul(Items);

        await Dbconn.ExecuteAsync(sqlinsert, data);

        





    }


}
