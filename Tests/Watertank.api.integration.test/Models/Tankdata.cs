using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Watertank.api.integration.test.Models
{

    public record Dbrecord(DateTime time, Guid tank_id, double current_volume, Guid? client_id = null, string? zone_code = null, double? total_capacity = null);

    public static class Tankdata
    {
        /// <summary>
        /// skipping days test  --->new (Add_Date(ref dateTime, 3)
        /// </summary>
        public static List<Dbrecord> TankItem1(Guid _tankid, string? _client_id  =null , string? _zonecode = null)
        {
            DateTime dateTime = default(DateTime).ResetDate();


            return new List<Dbrecord>
            {
                 new(dateTime, _tankid, 30,zone_code: _zonecode),
                 new(Add_Date(ref dateTime, 1) ,_tankid,30,zone_code:_zonecode),
                 new(Add_Date(ref dateTime, 1) ,_tankid,30,zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,28,zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,25,zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 3) ,_tankid,19,zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,15,zone_code:_zonecode)

            };
        }

        /// <summary>
        /// testing clientid and zonecode & multiple data same day with refill last hour of the day  
        /// </summary>
        public static List<Dbrecord> TankItem2(Guid _tankid, Guid? _client_id = null, string? _zonecode = null)
        {
            DateTime dateTime = default(DateTime).ResetDate();


            return new List<Dbrecord>
            {
                 new(dateTime, _tankid, 30,_client_id, _zonecode),
                 new(Add_Date(ref dateTime, 1,2 ) ,_tankid,28,_client_id,_zonecode),
                 new(Add_Date(ref dateTime, Hour:3) ,_tankid,25,_client_id, _zonecode),
                 new (Add_Date(ref dateTime, Hour:1) ,_tankid,35,_client_id,_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,30,_client_id,_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,24,_client_id, _zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,22,_client_id, _zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,19,_client_id, _zonecode)

            };
        }


        /// <summary>
        ///  GET multiple clientid from endpoint , refill mid day
        /// </summary>
        public static List<Dbrecord> TankItem3(Guid _tankid, Guid? _client_id = null, string? _zonecode = null)
        {
            DateTime dateTime = default(DateTime).ResetDate();


            return new List<Dbrecord>
            {
                 new(dateTime, _tankid, 30,_client_id),
                 new(Add_Date(ref dateTime, 1,2 ) ,_tankid,28,_client_id),
                 new(Add_Date(ref dateTime, Hour:3) ,_tankid,40,_client_id),
                 new (Add_Date(ref dateTime, Hour:1) ,_tankid,35,_client_id),
                 new (Add_Date(ref dateTime, 1) ,_tankid,30,_client_id),
                 new (Add_Date(ref dateTime, 1) ,_tankid,24,_client_id),
                 new (Add_Date(ref dateTime, 1) ,_tankid,22,_client_id),
                 new (Add_Date(ref dateTime, 1) ,_tankid,19,_client_id)

            };
        }


        /// <summary>
        ///  expired tankitem ,if the last volume is 0 the tank is automatically flagged as empty , return the last datetime  
        /// </summary>
        public static List<Dbrecord> TankItem4(Guid _tankid, Guid? _client_id = null, string? _zonecode = null)
        {
            DateTime dateTime = default(DateTime).ResetDate();


            return new List<Dbrecord>
            {
                  new(dateTime, _tankid, 30,zone_code: _zonecode),
                 new(Add_Date(ref dateTime, 1) ,_tankid,30),
                 new(Add_Date(ref dateTime, 1) ,_tankid,30),
                 new (Add_Date(ref dateTime, 1) ,_tankid,28),
                 new (Add_Date(ref dateTime, 1) ,_tankid,0),
                 new (Add_Date(ref dateTime, 1) ,_tankid,19),
                 new (Add_Date(ref dateTime, 1) ,_tankid,0)

            };
        }


        /// <summary>
        ///  template tankitem
        /// </summary>
        public static List<Dbrecord> TankItemx(Guid _tankid, Guid? _client_id = null, string? _zonecode = null)
        {
            DateTime dateTime = default(DateTime).ResetDate();


              return new List<Dbrecord>
            {
                 new(dateTime, _tankid, 30,zone_code: _zonecode),
                 new(Add_Date(ref dateTime, 1) ,_tankid,30,_client_id,zone_code:_zonecode),
                 new(Add_Date(ref dateTime, 1) ,_tankid,30,_client_id,zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,28,_client_id,zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,25,_client_id, zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,19,_client_id, zone_code:_zonecode),
                 new (Add_Date(ref dateTime, 1) ,_tankid,15,_client_id, zone_code:_zonecode)

            };
        
        }

       public static DateTime ResetDate(this DateTime x) => x = new(2025, 01, 01, 12, 0, 0);

        static DateTime Add_Date(ref DateTime date, int? Day = null, int? Hour = null)
        {


            if (Hour is not null && Hour > 0)
            {
                date = date.AddHours((int)Hour);
            }

            if (Day is not null && Day > 0)
            {
                date = date.AddDays((int)Day);
            }

            return date;
        }

    }
}
