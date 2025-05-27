using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using WaterTankMock_MQTT.Services;

namespace WaterTankMock_MQTT.ViewModels
{
    public class ViewModelBase : ObservableValidator
    {

        protected static  event Func<Page,Task>? CPage;

        protected static async Task Changepage(Page _page)
        {
            await (CPage?.Invoke(_page) ??  Task.CompletedTask); 
                
        }



    }
}
