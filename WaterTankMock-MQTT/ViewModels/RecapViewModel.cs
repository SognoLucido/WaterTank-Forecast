

using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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









      

        public





        //[ObservableProperty]
        //private ObservableCollection<TankItem>? _ztanks  =
        //      [
        //          new()
        //        {
        //            Id = Guid.NewGuid(),
        //            Capacity = 500,
        //            CurrentLevel = 200,
        //            Name = "Name",

        //        },
        //          new()
        //        {
        //            Id = Guid.NewGuid(),
        //            Capacity = 600,
        //            CurrentLevel = 400,
        //            Name = "Name1",
        //        },
        //            new()
        //        {
        //            Id = Guid.NewGuid(),
        //            Capacity = 700,
        //            CurrentLevel = 300,
        //             Name = "Name2",
        //        }


        //      ];



    }
}
