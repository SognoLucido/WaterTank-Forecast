using System;

namespace WaterTankMock_MQTT.Models
{
    public class RecapTankItemModel
    {

        public Guid Id { get; set; }

        public int Capacity { get; set; }

        public int CurrentLevel { get; set; }

        public string Name { get; set; }


        public bool Trigger1 { get; set; }
        public bool Trigger2 { get; set; }

        public bool Trigger3 { get; set; }
        public bool Trigger4 { get; set; }

        public bool Trigger5 { get; set; } = true;

        public bool Trigger6 { get; set; }

        public bool Trigger7 { get; set; }

        public bool Trigger8 { get; set; }

        public bool Trigger9 { get; set; }

        public bool Trigger10 { get; set; }
    }
}
