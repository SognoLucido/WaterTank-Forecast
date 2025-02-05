using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DataIngestAPI.Models
{
    public record WaterTankItem(
        DateTime timestamp , 
        Guid tank_id , 
        Double current_volume,
        Guid? client_id,
        string? zone_code,
        Double? total_capacity

        );

    [JsonSerializable(typeof(WaterTankItem))]
    public partial class WaterTankItemSGjsonConverter : JsonSerializerContext { }


}
