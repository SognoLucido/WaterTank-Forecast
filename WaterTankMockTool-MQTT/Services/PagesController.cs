
using Avalonia;
using Avalonia.Data;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
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
        Options,
        Sim,
        Start
    }

    public partial class PagesController : ViewModelBase 
    {

        private readonly IServiceProvider serviceProvider;
       // private ViewModelBase[] PageCollection { get; set; } = new ViewModelBase[5];
        public PagesController(IServiceProvider _serviceProvider/*,SettingsTankViewModel Tankset,RecapViewModel recap,OptionsViewModel options*/ /*,SimViewModel sim , StartViewModel start*/) 
        {

            serviceProvider = _serviceProvider;
            CPage += PageHandler;


           // PageCollection[0] = Tankset;
          //  PageCollection[1] = recap;
          //  PageCollection[2] = options;
           // PageCollection[3] = sim;
            //PageCollection[4] = start;
            
        }

        [ObservableProperty]
        private ViewModelBase? _pagez;




        private async Task PageHandler(Page pagename)
        {

            Pagez = pagename switch
            {
                Page.Null => null,
                //Page.TankSettings or
                //Page.Options or
                ////Page.Start or
                //Page.Recap => PageCollection[(int)pagename],
                Page.TankSettings => serviceProvider.GetRequiredService<SettingsTankViewModel>(),
                Page.Options => serviceProvider.GetRequiredService<OptionsViewModel>(),
                Page.Recap => serviceProvider.GetRequiredService<RecapViewModel>(),
                Page.Start => serviceProvider.GetRequiredService<StartViewModel>(),
                Page.Sim => serviceProvider.GetRequiredService<SimViewModel>(),

                _ => throw new ArgumentOutOfRangeException("Invalid page index")


            };

        }



        //public async Task Changepage(Page pagename)
        //{

        //    Pagez = pagename switch
        //    {
        //        Page.Null  => null,
        //        Page.TankSettings or
        //        Page.Options or
        //        //Page.Start or
        //        Page.Recap => PageCollection[(int)pagename],
        //        Page.Start => serviceProvider.GetRequiredService<StartViewModel>(),
        //        Page.Sim => serviceProvider.GetRequiredService<SimViewModel>(),

        //       _ => throw new ArgumentOutOfRangeException("Invalid page index")


        //    };

      


        //}


    }
}
