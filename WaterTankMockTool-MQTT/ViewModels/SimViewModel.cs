using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.Services.Mqtt;
using WaterTankMockTool_MQTT.Services;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class SimViewModel : ViewModelBase
    {

        private readonly Sharedata Sharedata;
        private readonly MqttInit mqttInit;
        public SimViewModel() { }

        private string _enddatetextInfo = "End Date: ----";
        private string? _datepicker;
        private string? _days;

     
        public SimViewModel(Sharedata _sharedata, MqttInit _mqtt)
        {
            Sharedata = _sharedata;
            mqttInit = _mqtt;

        }



        public string EnddatetextInfo
        {
            get => _enddatetextInfo;
            set
            {
                SetProperty(ref _enddatetextInfo, value);

            }

        }


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
                    if (Days is not null) Days = _days;

                }


            }
        }


        [Required(ErrorMessage = "The 'Day' field is required.")]
        [RangeStringtoInt(1, 100, ErrorMessage = "The 'Day' value must be between 1 and 100.")]
        public string? Days
        {
            get => _days;
            set
            {

                SetProperty(ref _days, value,true); // `true` triggers validation
                                               //   DayErrors = GetErrorsAsString(nameof(Days));

               // ValidateProperty(value, nameof(Days));

                if (int.TryParse(value, out var pep))
                {
                    if (pep > 0) 
                    { 
                         EnddatetextInfo = "End Date: " + Sharedata.StartTestDate.AddDays(pep).ToString();
                    }
                else EnddatetextInfo = "End Date: ----";

                }
                else EnddatetextInfo = "End Date: ----";

                if (Date is null)  EnddatetextInfo = "End Date: ----";

            }
        }


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

            //if (Valid is null || !(bool)Valid) return;
            //if (_days <= 0) return;
            //if (Regex.IsMatch(_days, @"^\d+$")) return;

            if (Date is null) return;

            if(HasErrors) return;

            Sharedata.Toxdays = int.Parse(_days!);

            await Changepage(Page.Start);
            await mqttInit.Startsim();
        }

        [RelayCommand]
        private async Task Goback()
        {

            await Changepage(Page.Options);

        }


    }
}
