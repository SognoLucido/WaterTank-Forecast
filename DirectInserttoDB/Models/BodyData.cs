using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DirectInserttoDB.Models
{
    public class BodyData
    {
       private static Random rng = new();
       public Guid Id { get;  } = Guid.NewGuid();
       public DateTime Time { get; set; }
        public Double Lvl { get; private set; } = rng.Next(600, 800);

        Guid? client_id { get; set; }   
        string? zone_code { get; set; }
        Double? total_capacity { get; set; }

        public BodyData(/*DateTime _time*/) 
        {

            Time = new DateTime(2025, 01, 01, 12, 00, 00, DateTimeKind.Utc);

            //Time = _time;
        }

        public void  Scaledown()
        {
            Lvl -= rng.Next(1, 11);
        }

    }
}


