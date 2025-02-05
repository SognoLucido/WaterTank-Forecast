namespace DataFlow_ReadAPI.Models
{
    public class DBreturnData
    {
       public Guid tank_id { get; set; }

       // public DateTime empty_at_day { get; set; }

        private DateTime empty_at_day;  
        public DateOnly TankEmptydate => DateOnly.FromDateTime(empty_at_day);

        public Guid? client_id { get; set; }
        public string? zone_code { get; set; }
        public int? total_capacity { get; set; }
    }
}
