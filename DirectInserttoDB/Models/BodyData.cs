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
       public Guid Id { get; } = Guid.NewGuid();
       public DateTime Time { get; set; }
        public Double Lvl { get; private set; } 

        public Guid? Clientid { get; set; }   
        public string? Zcode { get; set; }
        public Double? Totcapacity { get; set; }

        public BodyData(/*DateTime _time*/) 
        {

            Time = new DateTime(2025, 01, 01, 12, 00, 00, DateTimeKind.Utc);
            Lvl = rng.Next(600, 800);
            Clientid = Guid.NewGuid();
            Zcode = rng.Next(2) == 0 ? "pep" : "gog";
            Totcapacity = Lvl + rng.Next(300);

            //Time = _time;
        }

        public void  Scaledown()
        {
            Lvl -= rng.Next(1, 11);
        }

    }
}


