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
        //private Pages pep { get; set; }

        //public Sharedata(Pages pages) => pep = pages;

        public event EventHandler<Page>? DataChanged ;


       /* [ObservableProperty]*/ private ObservableCollection<TankItem>? _items;

        public ObservableCollection<TankItem>? Items
        {
            get => _items;
            set
            {
                SetProperty(ref _items, value);

                //if (Items != null && Items.Any()) Itemsempty = true;   //DA FIXARE
                //else Itemsempty = false;

            }
        }

        [ObservableProperty] private bool _gotorecap = false;
        [ObservableProperty] private bool _itemsNotempty = false;

        public string MqttTopic { get; set; } = "watertank";

        [ObservableProperty] private bool _mqttbusy = false;

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

                    

                    SelectedTankItem.Triggerfiltered = [.. SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray()];


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
      
   


        public bool Seeddata = false;
        public bool Capacity = false;
        public bool ClientidEnable = false;
        public bool ZonecodeEnable = false;



       




        // private string _clientguid = new Guid().ToString();


        public Guid Clientguid;
        public string ZonecodeID;


        //END client OPTIONs



        //START ZONECODE OPTIONs




        //partial void OnZoneCodeIDChanged(string value)
        //{
        //    ClearErrors(nameof(Input));

        //    // Try parsing
        //    if (!Guid.TryParse(value, out _))
        //    {
        //        // Trigger validation error
        //        SetErrors(nameof(Input), new[] { "Invalid GUID format." });
        //    }
        //}



        //END ZONECODE OPTIONs

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
