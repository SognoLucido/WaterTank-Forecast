using System;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
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

        public SimViewModel(Sharedata _sharedata, MqttInit _mqtt)
        {
            Sharedata = _sharedata;
            mqttInit = _mqtt;

           

            //BdMessg = build;
        }


        /* [ObservableProperty]*/
        private string _enddatetextInfo = "End Date: ----";

        public string EnddatetextInfo
        {
            get => _enddatetextInfo;
            set
            {
                SetProperty(ref _enddatetextInfo, value);

            }

        }




        //[ObservableProperty]
        private string? _datepicker;

        public string? Date
        {
            get => _datepicker;
            set
            {

                if (value is not null)
                    if (value.Contains('+') || value.Contains('-') && value.Length > 5) value = value.Substring(0, value.Length - 7);

                SetProperty(ref _datepicker, value);


                //if (value == null) Sharedata.StartTestDate = null;
                //else Sharedata.StartTestDate = DateTime.Parse(value);

                if (value != null)
                {

                    // var test = DateTime.UtcNow;

                    Sharedata.StartTestDate = DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Utc);

                    //if (Days is not null)  OnPropertyChanged(nameof(Days));
                    //if (Days is not null) Days = _days;

                }


            }
        }


        //public  int _days { get; set; } = 1;



        //BETTER VALIDATION TODO

        private int _days;
        public string? Days
        {
            get => _days.ToString();
            set
            {

                if(int.TryParse(value ,out var pep))   
                SetProperty(ref _days, pep);
                else
                {
                   
                    return;
                 
                }

                if (_days <= 0) return;
                // if(value is null) return;
                if (_datepicker is null) return;
                //if(value <= 0) throw new ArgumentNullException(nameof(Days), "Invalid input: The number must be greater than 0.");

                //OnPropertyChanged(nameof(Days));
                //if (value is not null)
                //{

                //   var x = Sharedata.StartTestDate.AddDays((int)value).ToString();

                EnddatetextInfo = "End Date: " + Sharedata.StartTestDate.AddDays(_days).ToString();
                //}

                //if (string.IsNullOrWhiteSpace(value))
                //{
                //    throw new ArgumentNullException(nameof(Days), "This field is required");
                //}

                //if (int.TryParse(value, out var numb))
                //{
                //    Sharedata.Toxdays = numb;
                //}
                //else
                //{
                //    throw new ArgumentException("Value must be a valid integer", nameof(Days));
                //}

                //OnPropertyChanged(nameof(Days));



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
            if (Days is null) return;

            testclean();
            Sharedata.Toxdays = _days;

            await Sharedata.Changepage(Page.Start);
            await mqttInit.Startsim();
        }

        [RelayCommand]
        private async Task Goback()
        {
            testclean();
            await Sharedata.Changepage(Page.Options);

        }

        //temp to fix
        private void testclean()
        {
            EnddatetextInfo = "End Date: ----";
        }

    }
}
