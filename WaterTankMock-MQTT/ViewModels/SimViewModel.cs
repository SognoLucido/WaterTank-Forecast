using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.Services.Mqtt;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class SimViewModel : ViewModelBase
    {

        private readonly Sharedata Sharedata;
        private readonly MqttInit mqttInit;
        public SimViewModel() { }

        public SimViewModel(Sharedata _sharedata,MqttInit _mqtt) 
        {
            Sharedata = _sharedata;
            mqttInit = _mqtt;
        }


        //[ObservableProperty]
        private string? _datepicker;

        public string? Date
        {
            get => _datepicker;
            set
            {
                SetProperty(ref _datepicker, value);


                if (value == null) Sharedata.StartTestDate = null;
                else Sharedata.StartTestDate = DateTime.Parse(value);


                //Test = value;
            }
        }


        //public  int _days { get; set; } = 1;


        private string _days = "1";
        public string Days
        {
            get => _days;
            set
            {
                SetProperty(ref _days, value);

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(Days), "This field is required");
                }

                if (int.TryParse(value, out var numb))
                {
                    Sharedata.Toxdays = numb;
                }
                else
                {
                    throw new ArgumentException("Value must be a valid integer", nameof(Days));
                }

                OnPropertyChanged(nameof(Days));



            }
        }




        public string BdMessg { get; } =
            @"{
  ""tank_id"": ""guid"",
  ""timestamp"": ""2024-11-27 14:30:00"",
  ""capacity"": {
    ""current_volume"": 500,
    ""total_capacity"": 1000,
    ""unit"": ""liters"",
  }
}";




        [RelayCommand]
        private async Task Continue()
        {
            await Sharedata.Changepage(Page.Start);
            await mqttInit.Startsim();
        }

        [RelayCommand]
        private async Task Goback()       
        {

            await Sharedata.Changepage(Page.Options);

        }
    }
}
