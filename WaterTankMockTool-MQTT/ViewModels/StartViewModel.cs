using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services.Mqtt;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class StartViewModel : ViewModelBase
    {

        public  Sharedata Sharedata { get; }

        public StartViewModel() { }

        public StartViewModel(Sharedata _sharedata) 
        {
            Sharedata = _sharedata;
          
        }

    }
}
