
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        public Sharedata Sharedata { get; }

        public MainWindowViewModel() { }


        private readonly MqttInit Mqtt;
        const int Mqttdefport = 1883;


        public MainWindowViewModel(MqttInit _mqtt, Sharedata sharedata)
        {
            Sharedata = sharedata;
            Mqtt = _mqtt;
            Items = [];
            ItemSelected = false;
            ConnectionIP = "localhost";
            Statustext = "Disconnected";
            Mqtt.ConnectionStatus += HandleConnectionStatus;
            Togglesettingbuttonname = "Settings";
            Rangevisible = false;
        }


        //  List<string> animalist;

        //public ObservableCollection<string> Items { get; set; }

        //public ObservableCollection<TankItem> Items { get; set; } = [];

        [ObservableProperty] private string _connectionIP;
        [ObservableProperty] private string? _connectionPort;

        //private int? _connectionPort;
        //public int? ConnectionPort 
        //{
        //    get => _connectionPort;
        //    set
        //    {
        //        SetProperty(ref _connectionPort, value);
        //        if (value is not null && _connectionPort is not null) Watermarkport = null;

        //    }
        //}





        [ObservableProperty] private int _keytotriggers;
        [ObservableProperty]private bool _connected;
        [ObservableProperty]private string _statustext;
        [ObservableProperty] private bool _itemSelected ;
        [ObservableProperty] private string _togglesettingbuttonname;
        [ObservableProperty] private string? _settingerror ;
        [ObservableProperty] private bool _rangevisible ;
        [ObservableProperty] private bool _addandclearvisib = true;

        private bool _settings; 
        public bool Settings
        {
            get => _settings;
            set
            {
                SetProperty(ref _settings, value);
                if (value)
                {
                    //Cleandata();
                    //for(int i = 0; i < SelectedTankItem.Triggers.Count; i++)
                    //{
                    //    if (!SelectedTankItem.Triggers[i].Active) continue;

                    //   var test = new KeyValuePair<int, string>(i, SelectedTankItem.Triggers[i].Name);

                    //    SelectedTankItem.TriggerComboBoxItems.Add(test);
                    //}

                    SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(SelectedTankItem.Triggers.Where(x => x.Active== true).ToArray());

                    //SelectedTankItem.Triggercombobox = SelectedTankItem.Triggers.Select((item, index) => new { item, index }).Where(x=>x.item.Active == true).ToDictionary(k=> k.index , v => v.item.Name);

                    SelectedTankItem.Activetriggers = SelectedTankItem.Triggers.Count(x => x.Active);
                    Togglesettingbuttonname = "Triggers";
                }
                else Togglesettingbuttonname = "Settings";
            }
        }


       

        private ObservableCollection<TankItem>? Backuplist;

        [ObservableProperty]
        private ObservableCollection<TankItem>? _items;

        //public ObservableCollection<TankItem>? Items 
        //{ get 
        //    {

        //        return _items;
        //    }  
        //    private set 
        //    { 
        //        _items = value;

        //        //Filteritem = _items;



        //    } 
        //}



        private string? _search;

        public string? Search
        {
            get => _search;
            set
            {


                if(string.IsNullOrEmpty(_search) && !string.IsNullOrEmpty(value))
                {
                    
                    Backuplist = _items;
                    SetProperty(ref _search, value);
                }
                else
                {
                    SetProperty(ref _search, value);
                }


               

                if (!string.IsNullOrEmpty(value))
                {
                    Addandclearvisib = false;

                       var orderbysearch = Items.Where(a => a.Name.Contains(value));

                       Items = new ObservableCollection<TankItem>(orderbysearch);

                }
                else
                {
                    Items = Backuplist;
                    Addandclearvisib = true;
                }
                //if (!string.IsNullOrEmpty(_search) && Items?.Count > 0)
                //{


                //    var orderbysearch = Items.Where(a => a.Name.Contains(_search));

                //    Items = new ObservableCollection<TankItem>(orderbysearch);
                //    OnPropertyChanged(nameof(Items));

                //}
                //else if (Backuplist is not null)
                //{
                //    Items = Backuplist;
                //    Backuplist = null;
                //}




            }
        }








        //[ObservableProperty]
        private TriggerItem? _triggeritemselect;

        public TriggerItem? Triggeritemselect
        {
            get
            {
                //if (_triggeritemselect == null) 
                //{
                //    _number1 = "";
                //    _number2 = "";
                //}
                
                return _triggeritemselect;
            }
            set
            {
                SetProperty(ref _triggeritemselect, value);


                if (value is not null) Rangevisible = true;
                else Rangevisible = false;

                
            }
        }


        //[ObservableProperty]private ObservableCollection<TankItem> _filteritem;

        //[ObservableProperty] private TankItem _selectedTankItem;

        private TankItem? _selectedTankItem;
        public TankItem? SelectedTankItem
        {
            get => _selectedTankItem;
            set 
            {
                SetProperty(ref _selectedTankItem, value);

                 Cleandata();

                if (_selectedTankItem is not null && value is not null)
                {
                    ItemSelected = true;
                    SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray());
                    

                }

                //page appear if the value is selected else blank

                if(value is null) ItemSelected = false;


                
            }
        }



        [ObservableProperty]
        private string? _number1;


        [ObservableProperty]
        private string? _number2;



        [RelayCommand]
        private async Task Saverange()
        {

            if (!int.TryParse(_number1, out int n1) || !int.TryParse(_number2, out int n2))
            {
                Settingerror = "Invalid Range";
                return;
            }
            else if (n1 < 0 || n2 < 0) 
            {
                Settingerror = "Error numbers nx<=0";
                return;
            }
            else
            {
                if (n1 > n2)
                {
                    Triggeritemselect.Rangemin = n2;
                    Triggeritemselect.Rangemax = n1;
                }
                else
                {
                    Triggeritemselect.Rangemin = n1;
                    Triggeritemselect.Rangemax = n2;
                }
                

            }
            
            Cleandata();
            Settingerror = string.Empty;
        }


        
        private async Task  Cleandata()
        {
            Number1 = "";
            Number2 = "";
            Triggeritemselect = null;
            Settingerror = string.Empty;



        }


        [RelayCommand]
        private async Task ClearAll()
        {


            var starredlist = Items.Where(x => x.Starring == true).ToArray();

            Items.Clear();

            Items = new ObservableCollection<TankItem>(starredlist);

            //OnPropertyChanged(nameof(Items));


        }



        [RelayCommand]
        private async Task AddItem()
        {
            Random rnd = new Random();

            var newGuid = Guid.NewGuid();

            var Newitem = new TankItem
            {
                Name =  newGuid.ToString()[..5] + "-TankItem",
                Id = newGuid,
                Capacity = rnd.Next(500,1001) ,
               
            };

            Newitem.CurrentLevel =  rnd.Next(Newitem.Capacity / 2 , Newitem.Capacity);


            Newitem.Starring = false;

            Items.Add(Newitem);

            //Backuplist = Items;


        }


        [RelayCommand]
        private async Task Testconnection()
        {


            if (!int.TryParse(ConnectionPort, out int port))
            {
                port = Mqttdefport;
            }


             Mqtt.Checkconnection(ConnectionIP,port);



        }


        private void HandleConnectionStatus(object? sender, bool isConnected)
        {
            if (isConnected)
            {
                Statustext = "Connected";
                Connected = true;
            }
            else
            {
                Statustext = "Disconnected";
                Connected = false;
            }
        }


        [RelayCommand]
        private async Task Star()
        {
            if(_itemSelected && _selectedTankItem is not null)
            {
                SelectedTankItem.Starring = !SelectedTankItem.Starring;


                Items = new ObservableCollection<TankItem>(Items.OrderByDescending(item => item.Starring));
                OnPropertyChanged(nameof(Items));
            }

        }






        [RelayCommand]
        private async Task Tests()
        {
           
            


        }







        //[RelayCommand]
        //private async Task Sendhttp()
        //{
        //    if (await testClass1.Sendyoz())
        //    {
        //        Textboxuser = "INVIO RIUSCITO ";
        //    }
        //    else
        //    {
        //        Textboxuser = "INVIO NON RIUSCITO";
        //    }
        //}





        //tuple

        



    }
}
