
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.ViewModels;

namespace WaterTankMock_MQTT.Services
{

   public enum Page
    {
        Null = -1,
        TankSettings = 0,
        Recap,
        Options
    }

    public partial class PagesController : ViewModelBase 
    {

 
        private ViewModelBase[] PageCollection { get; set; } = new ViewModelBase[3];
        public PagesController(SettingsTankViewModel Tankset,RecapViewModel recap,OptionsViewModel options) 
        {
            PageCollection[0] = Tankset;
            PageCollection[1] = recap;
            PageCollection[2] = options;
        }

        [ObservableProperty]
        private ViewModelBase? _pagez;


        public async Task Changepage(Page pagename)
        {
          

            Pagez = pagename == Page.Null ? null : PageCollection[(int)pagename];


        }


    }
}
