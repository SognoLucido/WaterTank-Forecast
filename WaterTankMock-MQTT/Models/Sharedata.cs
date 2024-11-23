using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.ViewModels;

namespace WaterTankMock_MQTT.Models
{
    public partial class Sharedata : ViewModelBase
    {
        //private Pages pep { get; set; }

        //public Sharedata(Pages pages) => pep = pages;

        public event EventHandler<Page>? DataChanged ;


        [ObservableProperty]
        private ObservableCollection<TankItem>? _items;

        [ObservableProperty] private bool _gotorecap = false;


        private TankItem? _selectedTankItem;
        public TankItem? SelectedTankItem
        {
            get => _selectedTankItem;
            set
            {
                SetProperty(ref _selectedTankItem, value);

                //Cleandata();

                if (_selectedTankItem is not null && value is not null)
                {

                    

                    SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray());


                    DataChanged?.Invoke(this, Page.TankSettings);
                    //Pages.Changepage(Page.TankSettings);
                    //ItemSelected = true;
                    //SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray());


                }
                



                if(value is null) DataChanged?.Invoke(this, Page.Null); ;

                //page appear if the value is selected else blank

                //if (value is null) ItemSelected = false;

            }

        }


        public async Task Changepage(Page p)
        {
            DataChanged?.Invoke(this, p);
        }

    }
}
