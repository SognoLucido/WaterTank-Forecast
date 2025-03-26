namespace DataFlow_ReadAPI.Models
{


    public class DBreturnDataDto
    {
       public int RangeDays { get; set; }
       public  Guid? Clientid { get; set; }
       public string? Zonecode { get; set; }
       public IEnumerable<DbforecastreturnData>? Data { get; set; }
    }



    public class DbforecastreturnData
    {
       public Guid tank_id { get; set; }

        public DateTime empty_at_day { get; set; }


      //  public DateOnly TankEmptydate => DateOnly.FromDateTime(empty_at_day);


        //public Guid? client_id { get; set; }
        //public string? zone_code { get; set; }
        //public int? total_capacity { get; set; }
    }
}
