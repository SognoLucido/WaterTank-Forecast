using System.Text.Json.Serialization;

namespace DataFlow_ReadAPI.Models
{
    public class DbInfoItem
    {
        public DateTime time { get; set; }
        public Guid tank_id { get; set; }
        public Double current_volume {get;set;}

        public Guid? client_id { get; set; }
      //  public string? Client_id_info { get; set; }

  
        public string? zone_code { get; set; }
      //  public string? Zone_code_info { get; set; }
     

        public double? total_capacity { get; set; }
       // public string? Total_capacity_info { get; set; }
    }
}
