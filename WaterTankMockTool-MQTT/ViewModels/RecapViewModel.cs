

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class RecapViewModel : ViewModelBase
    {
        public Sharedata Sharedata { get; }


        public RecapViewModel() { }


        public RecapViewModel(Sharedata sharedata )
        {
         
            Sharedata = sharedata;
        }


        [RelayCommand]
        private async Task Goback()
        {
           
            if(Sharedata.SelectedTankItem is null)
            {
                await Changepage(Page.Null);
            }
            else await Changepage(Page.TankSettings);

            Sharedata.Gotorecap = true;

        }



        [RelayCommand]
        private async Task Continue()
        {
            await Changepage(Page.Options);
        }






    }
}
