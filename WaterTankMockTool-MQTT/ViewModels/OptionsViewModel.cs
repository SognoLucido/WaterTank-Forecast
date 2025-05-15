using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;
using WaterTankMockTool_MQTT.Services;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class OptionsViewModel : ViewModelBase
    {

        public Sharedata Sharedata { get; }

        public OptionsViewModel() { }

        public OptionsViewModel(Sharedata sharedata)
        {
            Sharedata = sharedata;
        }

        private string _formClientId;
        private string? _topicname;
        private bool _formclientidenable = false;
        private bool _Formzonecodenable;
      

        //Toggle butt Enable
        [ObservableProperty] private bool _Formseeddata = false;
        //Toggle butt Enable
        [ObservableProperty] private bool _Formcapacity = false;
        [ObservableProperty] private string _clienttext = "Enable \"Client ID\"";
        [ObservableProperty] private int _clientfont = 20;
        [ObservableProperty] private string? _zonetext = "Enable \"Zone Code\"";

        [MaxLength(10,ErrorMessage ="Invalid Zonecode - Max Lenght 10")]
        [ObservableProperty] private string? _FormzonecodeID;











        [Guid(ErrorMessage = "Invalid Guid")]
        public string FormClientGuid
        {
            get => _formClientId;
            set
            {
                SetProperty(ref _formClientId, value, true);

                if (HasErrors)
                {
                    ClearErrors(nameof(FormClientGuid));
                }

            }
        }




        public string? Topicname
        {

            get => _topicname;
            set
            {
                SetProperty(ref _topicname, value);

                Sharedata.MqttTopic = value ?? "watertank";
            }
        }


        //TOGGLE CLIENT GUID
        public bool FormClientidEnable
        {
            get => _formclientidenable;
            set
            {
                SetProperty(ref _formclientidenable, value);

                if (value)
                {
                    Clientfont = 17;
                    Clienttext = "Client ID";
                }
                else
                {
                    Clientfont = 20;
                    Clienttext = "Enable \"Client ID\"";
                }

            }
        }

        //[ObservableProperty]

        //togle enable
        public bool FormZonecodeEnable
        {
            get => _Formzonecodenable;
            set
            {
                SetProperty(ref _Formzonecodenable, value);

                if (value)
                {

                    Zonetext = "Zone Code";
                }
                else
                {

                    Zonetext = "Enable \"Zone Code\"";
                }

            }
        }











        private void SaveDatatoShareclass()
        {

            Sharedata.Seeddata = Formseeddata;
            Sharedata.Capacity = Formcapacity;
            if (FormClientidEnable)
            {
                Sharedata.ClientidEnable = true;
                Sharedata.Clientguid = Guid.Parse(FormClientGuid);
            }
            if (FormZonecodeEnable)
            {
                Sharedata.ZonecodeEnable = true;
                Sharedata.ZonecodeID = FormzonecodeID!;
            }


        }





        [RelayCommand]
        private async Task GenerateNewGuid() => FormClientGuid = Guid.NewGuid().ToString();


        [RelayCommand]
        private async Task Goback()
        {

            await Sharedata.Changepage(Page.Recap);

        }



        [RelayCommand]
        private async Task Continue()
        {
            ValidateAllProperties();
            if (!HasErrors)
            {
                SaveDatatoShareclass();
                await Sharedata.Changepage(Page.Sim);

            }
        }














    }
}
