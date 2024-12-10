using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterTankMock_MQTT.Services.Mqtt.Models
{
    public class MqttBodyJsonModel
    {
         public Guid tank_id { get; set; }
        public DateTime timestamp { get; set; }
        public Capacity capacity { get; set; }
    }

    public class Capacity
    {
        public int current_volume { get; set; }
        public int total_capacity { get; set; }
        public string unit { get; set; } = "liters";
    }
}
