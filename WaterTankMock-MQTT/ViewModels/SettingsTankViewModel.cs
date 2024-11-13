using CommunityToolkit.Mvvm.ComponentModel;


namespace WaterTankMock_MQTT.ViewModels
{
    public partial class SettingsTankViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _title = "greetings";

    }
}
