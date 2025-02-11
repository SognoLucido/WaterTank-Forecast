using Avalonia.Metadata;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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


           
            //BdMessg = build;
        }


        //[ObservableProperty]
        private string? _datepicker;

        public string? Date
        {
            get => _datepicker;
            set
            {

               if(value is not null)
                    if (value.Contains('+') || value.Contains('-') && value.Length > 5) value = value.Substring(0, value.Length - 7);

                SetProperty(ref _datepicker, value);


                //if (value == null) Sharedata.StartTestDate = null;
                //else Sharedata.StartTestDate = DateTime.Parse(value);

                if (value != null) 
                {

                    var test = DateTime.UtcNow;

                    

                    Sharedata.StartTestDate  = DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Utc);
                                 
                }
               

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




        //        public string BdMessg { get; } =
        //        @"{
        //""tank_id"": ""guid"",
        //""timestamp"": ""2024-11-27 14:30:00"",
        //""current_volume"": 500,
        //""total_capacity"": 1000,
        //""client_id"": ""guid"",
        //""zone_code"": ""VARCHAR(10)""
        //}";

        public string BdMessg { get => Pepz(); } 


         private string Pepz()
        {
            StringBuilder sb = new("{\n\"tank_id\": \"GUID\",\n\"timestamp\": \"2024-11-27 14:30:00\",\n\"current_volume\": \"500\"");

            bool end = false;

            if (Sharedata.Capacity)
            {
                end = true;
                sb.AppendLine(",");
                sb.Append("\"total_capacity\": \"1000\"");
            }
            if (Sharedata.ClientidEnable)
            {
                end = true;
                sb.AppendLine(",");
                sb.Append("\"client_id\": \"GUID\"");
            }
            if (Sharedata.ZonecodeEnable)
            {
                end = true;
                sb.AppendLine(",");
                sb.Append("\"zone_code\": \"VARCHAR(10)\"");

            }

            if (!end) 
            {
                sb.Append(',');
            }
            sb.Append("\n}");

            return sb.ToString();
        }

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
