using System.Diagnostics;
using Dapper;
using DirectInserttoDB.Models;
using Npgsql;

namespace DirectInserttoDB;



//public class WaterTank
//{
//    public DateTime Time { get; set; }
//    public Guid Id { get; set; }
//    public double Lvl { get; set; }
//    public Guid? Clientid { get; set; }
//    public string? Zcode { get; set; }
//}


internal class DbCustomInsert
{
    readonly string Connstring;




    internal DbCustomInsert()
    {
        Connstring  = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword"; 
    }



    public async Task Start()
    {

        string? input = string.Empty;

        while (true)
        {
            Console.WriteLine("Insert one or more record/s.Every record must end with carriage return es.:\n" +
                "2025-01-01 12:00:00,C05471BC-7B77-442D-900F-55479B799444,500,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000\n" +
                "2025-01-02 12:00:00,C05471BC-7B77-442D-900F-55479B799444,450,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000\n" +
                "2025-01-03 12:00:00,C05471BC-7B77-442D-900F-55479B799444,400,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000\n" +
                "2025-01-04 12:00:00,C05471BC-7B77-442D-900F-55479B799444,350,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000\n\n" +

                "Insert-paste\n- Paste the custom Data in the console\n- Press Enter (go in a new line)\n- Press Ctrl+Z\n- Then press Enter\n\n(DateTime(YYYY-MM-DD HH:mm:SS(TimeOnly optional)),Guid tank_id,Double current_volume,Guid? client_id,string? zone_code(VARCHAR10),Double? total_capacity):"
                );



            //string input =
            //    "2025-01-01 12:00:00,C05471B-7B777-442D-900F-55479B799444,500,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000\n" +  //1 error wrong tankid guid
            //    "2025-01-02 12:00:00,C05471BC-7B77-442D-900F-55479B799444,450,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,\n" +     //2  OK, empty totalcap ; nullable
            //    "2025-01-02 12:00:00,C05471BC-7B77-442D-900F-55479B799444,450,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,hello\n" + //3 totalcap wrong pars
            //    "2025-01-03 12:00:00,C05471BC-7B77-442D-900F-55479B799444,400,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000\n" + // 4 OK
            //    "2025-01-04 12:00:00,C05471BC-7B77-442D-900F-55479B799444,,8C3BDB05-8277-4A45-93F5-DDBBE2235562,RC-002Z,1000";   //5 error no Currentlvl


            input = Console.In.ReadToEnd();


            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("invalid input\n");
            }
            else break;


        }

        var tanks = input
        .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(TankdataParse.Parse)

        .ToList();

        List<int> errosLines = new(tanks.Count);


        for (int i = 0; i < tanks.Count; i++)
        {

            if (tanks[i] is null)
            {
                errosLines.Add(i + 1);
            }

        }

        if (errosLines.Any())
        {
            Console.Write("\nError to parse these lines :");
            foreach (var item in errosLines)
            {
                if (item != 0) Console.Write(" " + item);
            }



        }

        Console.Write("\nFilter out unparsed items and proceed ?(y,exit): ");

        string? inputCS = Console.ReadLine();

        if (!string.IsNullOrEmpty(inputCS) && inputCS.Equals("y", StringComparison.CurrentCultureIgnoreCase))
        {

            tanks.RemoveAll(x => x is null);

        }
        else return;

        if (tanks.Count == 0) 
        {
            Console.WriteLine("nothing to send");
        }
        Console.Clear();
        Console.WriteLine("\nSeeding...");

     
        using var Dbconn = new NpgsqlConnection(Connstring);

        const string sqlinsert = @"INSERT INTO watertank (time, tank_id, current_volume, client_id, zone_code,total_capacity)
                                   VALUES (@Time, @TankId, @CurrentVolume, @ClientId, @ZoneCode, @TotalCapacity)";

        var Stime = Stopwatch.GetTimestamp();


        try{
            await Dbconn.ExecuteAsync(sqlinsert, tanks);
        }
        catch(Exception ex) {

            Console.WriteLine(ex.Message);
        }
       


        var timerange = Stopwatch.GetElapsedTime(Stime, Stopwatch.GetTimestamp());


        Console.WriteLine("ElapsedTime : " + timerange);

       

    }


}
