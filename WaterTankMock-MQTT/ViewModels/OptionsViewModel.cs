using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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







        [RelayCommand]
        private async Task Goback()
        {

   
                await Sharedata.Changepage(Page.Recap);
          
        }



        [RelayCommand]
        private async Task Continue()
        {
            await Sharedata.Changepage(Page.Options);
        }














    }
}
