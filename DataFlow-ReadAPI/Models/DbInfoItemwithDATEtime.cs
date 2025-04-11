using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DataFlow_ReadAPI.Models
{







    public class DbjsonparseDTO
    {
        public Guid tank_id { get; set; }
        public string Jsondata { get; set; } = "";
    }

    public class DbInfoItemwithDATEtime
    {
        public Guid tank_id { get; set; }
        public List<DateTimerange>? data { get; set; }
    }

    public class DateTimerange
    {
        
        public DateTime time { get; set; }
        public double current_volume { get; set; }
        public Guid? client_id { get; set; }

        //varchar10 
        public string? zone_code { get; set; }
        public double? total_capacity { get; set; }

    }

    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dateTimeOffset = DateTimeOffset.Parse(reader.GetString()!);
            return dateTimeOffset.UtcDateTime; 
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
        }
    }

}
