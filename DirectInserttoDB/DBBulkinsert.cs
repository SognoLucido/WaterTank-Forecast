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
    private int? Nrefills;
    private int? x100_ToRefill;
    private string? Zcode;
    private Guid? Client_id;
    private DateTime StartTime;
    private int ConsumptionRate;


    internal DBBulkinsert()
    {
        Connstring = "Host=localhost;Port=5432;Database=WaterTank;Username=postgres;Password=mypassword";
    }


    //settings
    public async Task Start()
    {
        string? input;

        while (true)
        {
            // Recap();
            Console.Clear();
            
            while (true)
            {

                Console.Write("Insert the number of days(forecastApp will not work well with less then 2 refill excluded)\nand the number of items, separated by a space. The total data entries will be calculated as days * items.\nFor example, 30 5 will create 5 items, each with 30 days of simulation \n (like 5 sensors sending data daily for 30 days) : (days item/s):");
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
                x100_ToRefill = default;

                Console.WriteLine("implement refills ? (y/n)");
                input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input) && input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    Enablerefills = true;


                    Console.WriteLine("(Automatic refill at x% of the resource)Input the limit % at which the AUTOMATIC refill should occur\n(e.g., at 30% of the total capacity, the refill will occur — input 30; leave blank to skip)\n(int?): ");
                    input = Console.ReadLine();


                    if (String.IsNullOrEmpty(input)) { }
                    else if (!int.TryParse(input, out int _inputRefull))
                    {
                        Console.WriteLine("invalid %refill");
                        continue;
                    }
                    else { x100_ToRefill = _inputRefull; }


                    Console.WriteLine($"Refills apply to random day choosen from the range 1 to maxdays:\nApply to all items, or some (x items will be randomly picked).\nEnter a number(must be <= of {items} item/s) or leave blank to skip (int?): ");
                    input = Console.ReadLine();

                    if (!int.TryParse(input, out int _inputNrefills))
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

                        Nrefills = _inputNrefills;

                    }


                }

                break;
            }

            if (Nrefills is null && x100_ToRefill is null) Enablerefills = false;


            while (true)
            {
                Console.WriteLine("Enable zonecode (all generated records will have a zcode if enabled).Leave empty to skip (VARCHAR10): ");
                input = Console.ReadLine();

                if (String.IsNullOrEmpty(input)) break;
                else
                {
                    if (input.Length > 10)
                    {
                        Console.WriteLine("invalid zcode lenght .max10");
                        continue;
                    }

                }

                Zcode = input;
                break;
            }

            while (true)
            {

                Console.WriteLine("Enable Client ID (all generated records will have a client id if enabled).(y/n): ");
                input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input) && input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
                {
                    Console.WriteLine("Insert a valid GUID or leave blank to generate one randomly (GUID?): ");
                    input = Console.ReadLine();

                    if (String.IsNullOrEmpty(input))
                    {
                        Client_id = Guid.NewGuid();

                    }
                    else
                    {
                        if (Guid.TryParse(input, out Guid tempGuid))
                        {
                            Client_id = tempGuid;

                        }
                        else
                        {
                            Console.WriteLine("invalid Guid");
                            continue;
                        }

                    }

                    if (Client_id is not null) Console.WriteLine($"client_Id is: {Client_id}");

                }
                else break;

                break;
            }

            while (true)
            {


                Console.WriteLine("Insert start time , Empty/Default = 2025-01-01 (YYYY-MM-DD)?:");
                input = Console.ReadLine();

                if (!String.IsNullOrEmpty(input))
                {
                    if (DateTime.TryParse(input, out StartTime)) break;
                    else continue;

                }
                else
                {
                    StartTime = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);
                    break;
                }


            }



            while (true)
            {
                Console.WriteLine("Insert consumption rate (the input number will be used to randomly scale daily consumption from 1 to the input value).Empty/default = 20 (int?):");
                input = Console.ReadLine();

                if (!String.IsNullOrEmpty(input))
                {
                    if (int.TryParse(input, out ConsumptionRate)) break;
                    else
                    {
                        Console.WriteLine("invalid input");
                        continue;
                    }

                }
                else
                {
                    ConsumptionRate = 20;

                }

                //the rng is max exclusive
                ++ConsumptionRate;

                break;

            }


            Recap();


            Console.WriteLine("Start seed - y , Redo the init - n : ");
            input = Console.ReadLine();

            if (!string.IsNullOrEmpty(input) && input.Equals("y", StringComparison.CurrentCultureIgnoreCase))
            {
                await Dblogic();
                break;
            }
            else Cleanup();
        }
    }


    private void Cleanup()
    {
        days = default;
        items = default;
        Enablerefills = default;
        Nrefills = default;
        x100_ToRefill = default;
        Zcode = default;
        Client_id = default;
        StartTime = default;
        ConsumptionRate = default;
    }



    private void Recap()
    {
        Console.Clear();
        Console.WriteLine("{0,-25}{1,-20}", "StartTime ", StartTime);
        Console.WriteLine("{0,-25}{1,-20}", "Day/s", days);
        Console.WriteLine("{0,-25}{1,-20}", "Item/s", items);
        Console.WriteLine("{0,-25}{1,-20}", "Totalcapacity(item)", "hardcoded->1000");
        Console.WriteLine("{0,-25}{1,-20}", "Currentcapacuty(item)", "hardcoded->[Totcapacity - rng.Next(201)]");
        Console.WriteLine("{0,-25}{1,-20}", "client_id", Client_id is null ? "Disabled/null" : Client_id);
        Console.WriteLine("{0,-25}{1,-20}", "zone_code", Zcode is null ? "Disabled/null" : Zcode);
        Console.WriteLine("{0,-25}{1,-20}", "ConsumptionRate", "1 ~ " + (ConsumptionRate - 1));
        Console.WriteLine("{0,-25}{1,-20}", "Enablerefills", Enablerefills);
        Console.WriteLine("{0,-25}{1,-20}", "Refill at %", x100_ToRefill is not null && Enablerefills ? x100_ToRefill : "disabled");
        Console.WriteLine("{0,-25}{1,-20}", "X Random day/s choosen to fill(x generated on obj init)", Nrefills is not null && Enablerefills ? Nrefills : "disabled");
        Console.WriteLine();
        // Console.WriteLine("{0,-8}{1,-12}{2,-20}{3,-14}{4,-14}{5,-14}", "time", "tank_id", "current_volume", "client_id", "zone_code", "total_capacity");//, "client_id", "zone_code", "total_capacity");


    }


    private int[] Generaterefillsdays(Random _rng)
    {




        int[] range = new int[(int)Nrefills!];

        for (int i = 0; i < range.Length;)
        {
            int randomN = _rng.Next(days);

            if (!range.Contains(randomN))
            {
                range[0] = randomN;
                i++;
            }


        }

        return range;
    }


    public async Task Dblogic()
    {

        Random rng = new();

        const string sqlinsert = @"INSERT INTO watertank (time ,tank_id, current_volume,client_id,zone_code,total_capacity)VALUES (@Time , @Id , @Lvl,@Clientid, @Zcode,@Totcapacity )";

        using var Dbconn = new NpgsqlConnection(Connstring);

        BodyData[] Items = new BodyData[items];

        for (int i = 0; i < Items.Length; i++)
        {
            Items[i] = new(StartTime, Client_id, Zcode, Enablerefills && Nrefills is not null ? Generaterefillsdays(rng) : null);
        }


        var Stime = Stopwatch.GetTimestamp();


        for (int i = 0; i < days; i++)
        {

            foreach (var data in Items)
            {
                if (i > 0)
                {
                    data.Time = data.Time.AddDays(1);

                    if (Enablerefills) data.Scaledown(in ConsumptionRate, in x100_ToRefill, in i);
                    else data.Scaledown(in ConsumptionRate);

                }


            }


            await Dbconn.ExecuteAsync(sqlinsert, Items);

            Console.WriteLine($"{i + 1}/{days} + batch inserted");



        }


        var timerange = Stopwatch.GetElapsedTime(Stime, Stopwatch.GetTimestamp());

        Console.WriteLine(timerange);


    }






}
