using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.ViewModels;
using WaterTankMockTool_MQTT.Services;

namespace WaterTankMock_MQTT.Models
{
    public partial class Sharedata : ViewModelBase
    {

        public string MqttTopic { get; set; } = "watertank";

        public bool Seeddata = false;
        public bool Capacity = false;
        public bool ClientidEnable = false;
        public bool ZonecodeEnable = false;
        public Guid Clientguid;
        public string? ZonecodeID;
        private TankItem? _selectedTankItem;
        private DateTime _startdate;

        //public ObservableCollection<TankItem>? Items
        //{
        //    get => _items;
        //    set
        //    {
        //        SetProperty(ref _items, value);

        //        //if (Items != null && Items.Any()) Itemsempty = true;   //DA FIXARE
        //        //else Itemsempty = false;

        //    }
        //}

        [ObservableProperty] private ObservableCollection<TankItem>? _items;
        [ObservableProperty] private bool _gotorecap = false;
        [ObservableProperty] private bool _itemsNotempty = false;
        [ObservableProperty] private bool _mqttbusy = false;
        [ObservableProperty] private string? _datatostringliveUpdate;

        [ObservableProperty] private string _progressMessage = string.Empty;
        [ObservableProperty] private int _progressBar = 0;
        [ObservableProperty] private ObservableCollection<bool> _simtriggers = [false, false, false, false, false, false, false, false, false, false];
        [ObservableProperty] private int _daycount = 0;
        [ObservableProperty] private int _toxdays = 1;

        public TankItem? SelectedTankItem
        {
            get => _selectedTankItem;
            set
            {
                SetProperty(ref _selectedTankItem, value);

                //Cleandata();

                if (_selectedTankItem is not null && value is not null)
                {

                    

                    SelectedTankItem.Triggerfiltered = [.. SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray()];


                   Changepage(Page.TankSettings);
                    //Pages.Changepage(Page.TankSettings);
                    //ItemSelected = true;
                    //SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray());


                }
                else { Changepage(Page.Null); }

                //page appear if the value is selected else blank

                //if (value is null) ItemSelected = false;

            }

        }

        public DateTime StartTestDate 
        { 
            get => _startdate;
            set 
            {
              
                
                SetProperty(ref _startdate, value);



                DatatostringliveUpdate = value.ToString();


            }
        }


      




    }
}
