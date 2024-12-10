using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;

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



        
        private string? _topicname ;

        public string? Topicname
        {

        get => _topicname; 
            set 
            {
                SetProperty(ref _topicname, value);

                Sharedata.MqttTopic = value ?? "watertank";
            }
       }



        [RelayCommand]
        private async Task Goback()
        {

            await Sharedata.Changepage(Page.Recap);

        }



        [RelayCommand]
        private async Task Continue()
        {
            await Sharedata.Changepage(Page.Sim);
        }














    }
}
