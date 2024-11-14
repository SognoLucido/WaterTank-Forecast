
using CommunityToolkit.Mvvm.ComponentModel;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.ViewModels;

namespace WaterTankMock_MQTT.Services
{

   public enum Page
    {
        Null = -1,
        TankSettings = 0,
        StartTest
    }



    public partial class PagesController : ViewModelBase 
    {

 
        private ViewModelBase[] PageCollection { get; set; } = new ViewModelBase[2];
        public PagesController(SettingsTankViewModel Tankset) 
        {
            PageCollection[0] = Tankset;

        }

        [ObservableProperty]
        private ViewModelBase? _pagez;


        public void Changepage(Page pagename)
        {
          

            Pagez = pagename == Page.Null ? null : PageCollection[(int)pagename];




        }


    }
}
