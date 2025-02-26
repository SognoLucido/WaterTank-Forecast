namespace DataFlow_ReadAPI.Models
{
    public class DbDataInfoItem
    {
        public Guid Tank_id { get; set; }
        public Double Current_volume {get;set;}
        public Guid? Client_id { get; set; }
        public string? Zone_code { get; set; }
        public int? Total_capacity { get; set; }
    }
}
