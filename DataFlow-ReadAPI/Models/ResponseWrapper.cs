namespace DataFlow_ReadAPI.Models
{
    public class ResponseWrapper
    {
        public DbInfoItem[]? WithoutDateRange { get; set; }
        public DbInfoItemwithDATEtime[] WithDateRange { get; set; }
    }
}
