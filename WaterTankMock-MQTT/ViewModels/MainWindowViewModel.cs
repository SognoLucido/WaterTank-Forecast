
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
        public PagesController Pages { get; }



        public MainWindowViewModel() { }


        private readonly MqttInit Mqtt;
        const int Mqttdefport = 1883;


        public MainWindowViewModel(MqttInit _mqtt, Sharedata sharedata,PagesController pages)
        {
  
            Pages = pages;
            Sharedata = sharedata;
            Sharedata.DataChanged += Sharedata_PageItemselected;
            Mqtt = _mqtt;
            Sharedata.Items = [];
            ConnectionIP = "localhost";
            Statustext = "Disconnected";
            Mqtt.ConnectionStatus += HandleConnectionStatus;
           
        }

        private void HandleConnectionStatus(object? sender, bool isConnected)
        {
            if (isConnected)
            {
                Statustext = "Connected";
                Connected = true;
                Pages.Changepage(Page.Recap);
            }
            else
            {
                Statustext = "Disconnected";
                Connected = false;
                Loadingbar = false;
            }
           
        }

        private void Sharedata_PageItemselected(object? sender, Page e)
        {
            Pages.Changepage(e);
        }



        [ObservableProperty] private string _connectionIP;
        [ObservableProperty] private string? _connectionPort;

    


        [ObservableProperty] private int _keytotriggers;
        [ObservableProperty]private bool _connected;
        [ObservableProperty]private string _statustext;
        [ObservableProperty] private bool _loadingbar = false;
       
        [ObservableProperty] private bool _addandclearvisib = true;

     

       

        private ObservableCollection<TankItem>? Backuplist;





        private string? _search;

        public string? Search
        {
            get => _search;
            set
            {


                if(string.IsNullOrEmpty(_search) && !string.IsNullOrEmpty(value))
                {
                    
                    Backuplist = Sharedata.Items;
                    SetProperty(ref _search, value);
                }
                else
                {
                    SetProperty(ref _search, value);
                }


               

                if (!string.IsNullOrEmpty(value))
                {
                    Addandclearvisib = false;

                       var orderbysearch = Sharedata.Items.Where(a => a.Name.Contains(value));

                    Sharedata.Items = new ObservableCollection<TankItem>(orderbysearch);

                }
                else
                {
                    Sharedata.Items = Backuplist;
                    Addandclearvisib = true;
                }
              



            }
        }





        [RelayCommand]
        private async Task ClearAll()
        {


            var starredlist = Sharedata.Items.Where(x => x.Starring == true).ToArray();

            Sharedata.Items.Clear();

            Sharedata.Items = new ObservableCollection<TankItem>(starredlist);

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

            Sharedata.Items.Add(Newitem);

            //Backuplist = Items;


        }


        [RelayCommand]
        private async Task Testconnection()
        {
            Loadingbar = true;
           
            if (!int.TryParse(ConnectionPort, out int port))
            {
                port = Mqttdefport;
            }


             Mqtt.Checkconnection(ConnectionIP,port);



        }


      





        [RelayCommand]
        private async Task Tests()
        {
           
            


        }









    }
}
