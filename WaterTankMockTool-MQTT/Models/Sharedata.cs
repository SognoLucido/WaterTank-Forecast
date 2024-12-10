using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
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

        public string MqttTopic { get; set; } = "watertank";


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


        /////sim settings 
      
        [ObservableProperty]
        private bool _seeddata = true;


        [ObservableProperty]
        private string? _datatostringliveUpdate;



        private DateTime _startdate;
        public DateTime StartTestDate 
        { 
            get => _startdate;
            set 
            {
                
                
                SetProperty(ref _startdate, value);



                DatatostringliveUpdate = value.ToString();


            }
        }

        [ObservableProperty]
        private string _progressMessage = string.Empty;

        [ObservableProperty]
        private int _progressBar = 0;

        [ObservableProperty]
        private ObservableCollection<bool> _simtriggers = [false, false, false,false,false,false,false,false,false,false];

        [ObservableProperty]
        private int _daycount = 0;

        [ObservableProperty]
        private int _toxdays = 1;

    }
}
