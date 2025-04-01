using System.Diagnostics;
using Dapper;
using DirectInserttoDB.Models;
using Npgsql;

namespace DirectInserttoDB;

internal class DBBulkinsert
{
    private string Connstring;
    private int days;
    private int items;
    private bool Enablerefills;
    private int Nrefills;
    private int x100_ToRefill;


    internal DBBulkinsert()
    {
        Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";
    }


    //settings
    public async Task Start()
    {
        Console.Clear();
        string? input;
        while (true)
        {

            Console.Write("Insert the number of days(forecastApp will not work well with less then 2 refill excluded) and the number of items, separated by a space. The total data entries will be calculated as days * items. For example, 30 5 will create 5 items, each with 30 days of simulation (like a sensor sending data daily for 30 days) : (days item/s):");
            input = Console.ReadLine();

            if (String.IsNullOrEmpty(input))
            {
                Console.WriteLine("invalid input string");
                continue;
            }

            var data = input.Split(' ');

            if (data.Length != 2)
            {
                Console.WriteLine("invalid param numbers");
                continue;
            }


            if (!int.TryParse(data[0], out days))
            {
                Console.WriteLine("invalid days param");
                continue;
            }
            if (!int.TryParse(data[1], out items))
            {
                Console.WriteLine("invalid items param");
                continue;
            }

            break;
        }

        while (true)
        {
            Enablerefills = default;
            Nrefills = default;
            x100_ToRefill = 50;

            Console.WriteLine("implement refills ? (y/n)");
            input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input) && input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
            {
                Enablerefills = true;

                Console.WriteLine($"Refills: Apply to all items, or some (x items will be randomly picked). Enter a number(must be <= of {items} item/s) or leave blank to skip (int?): ");
                input = Console.ReadLine();

                if (!int.TryParse(input, out Nrefills))
                {
                    Console.WriteLine("invalid N refill");
                    continue;
                }
                else
                {
                    if (Nrefills > items)
                    {
                        Console.WriteLine("invalid N refill");
                        continue;
                    }

                }

                Console.WriteLine("Input the limit % at which the refill should occur (e.g., at 30% of the total capacity, the refill will occur — input 30; default = 50) (int?): ");
                input = Console.ReadLine();


                if (String.IsNullOrEmpty(input)) { }
                else if (!int.TryParse(input, out x100_ToRefill))
                {
                    Console.WriteLine("invalid %refill");
                    continue;
                }




            }
        }
    }

    public async Task Dblogic(int itemX)
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

            Console.WriteLine($"{i + 1}/{days} + batch inserted");



        }


        var timerange = Stopwatch.GetElapsedTime(Stime, Stopwatch.GetTimestamp());

        Console.WriteLine(timerange);


    }
}
