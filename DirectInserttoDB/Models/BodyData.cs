namespace DirectInserttoDB.Models
{
    public class BodyData
    {
        private static Random rng = new();
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime Time { get; set; }
        public Double Lvl { get; private set; }

        public Guid? Clientid { get; set; }
        public string? Zcode { get; set; }
        public Double? Totcapacity { get; set; }

        private readonly int[]? WhenRefillDays;

        public BodyData(DateTime _time, Guid? _client_id, string? _zcode, int[]? _whenRefill_occur)
        {
            WhenRefillDays = _whenRefill_occur;
            // Time = new DateTime(2025, 01, 01, 12, 00, 00, DateTimeKind.Utc);
            Time = new DateTime(_time.Year, _time.Day, _time.Hour, 12, 00, 00, DateTimeKind.Unspecified);
            Totcapacity = 1000;
            Lvl = (double)Totcapacity - rng.Next(201);
            Clientid = _client_id;
            Zcode = _zcode;



            //Time = _time;
        }

        public void Scaledown(in int _max_consumption)
        {
            Lvl -= rng.Next(1, _max_consumption);
        }


        public void Scaledown(in int _max_consumption, in int? _refills_x100, in int _currentday)
        {

            var TotalCap = (double)Totcapacity!;


            if (_refills_x100 is not null)
            {

                double lvltoRefill = (double)(Totcapacity! * _refills_x100 / 100);

                if ((int)Lvl <= lvltoRefill)
                {
                    Lvl = TotalCap;
                    return;
                }
            }

            if (WhenRefillDays is not null)
            {
                if (WhenRefillDays!.Contains(_currentday))
                {
                    Lvl = TotalCap;
                    return;
                }
            }


            Lvl -= rng.Next(1, _max_consumption);
        }



    }
}


/*
 * 
 * 
 * 
 * 
 * 
 * x  private string Connstring;
   x private int days;  
   x private int items;  
   x private bool Enablerefills;
   x private int Nrefills;
    private int x100_ToRefill;
   x private string Zcode;
   x private Guid? Client_id;
   x private DateTime StartTime;
   x private int ConsumptionRate;
*/