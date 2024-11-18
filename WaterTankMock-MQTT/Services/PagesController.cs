
using CommunityToolkit.Mvvm.ComponentModel;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.ViewModels;

namespace WaterTankMock_MQTT.Services
{

   public enum Page
    {
        Null = -1,
        TankSettings = 0,
        Recap
    }

    public partial class PagesController : ViewModelBase 
    {

 
        private ViewModelBase[] PageCollection { get; set; } = new ViewModelBase[2];
        public PagesController(SettingsTankViewModel Tankset,RecapViewModel recap) 
        {
            PageCollection[0] = Tankset;
            PageCollection[1] = recap;
        }

        [ObservableProperty]
        private ViewModelBase? _pagez;


        public void Changepage(Page pagename)
        {
          

            Pagez = pagename == Page.Null ? null : PageCollection[(int)pagename];




        }


    }
}
