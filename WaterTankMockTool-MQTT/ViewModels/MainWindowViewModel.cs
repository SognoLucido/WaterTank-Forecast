
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterTankMock_MQTT.Models;
using WaterTankMock_MQTT.Services;
using WaterTankMock_MQTT.Services.Mqtt;

namespace WaterTankMock_MQTT.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {

        public Sharedata Sharedata { get; }
        public  PagesController Pages { get; }

        private CancellationTokenSource? _cTokenDisconnectfrommqtt;

        public MainWindowViewModel() { }


        private readonly MqttInit Mqtt;
        const int Mqttdefport = 1883;



        public MainWindowViewModel(MqttInit _mqtt, Sharedata sharedata, PagesController pages)
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

        private async void HandleConnectionStatus(object? sender, bool isConnected)
        {
            if (isConnected)
            {
                Statustext = "Connected";
                await Pages.Changepage(Page.Recap);
                Connected = true;

            }
            else
            {
                Statustext = "Disconnected";
                Connected = false;
                Loadingbar = false;
                Sharedata.SelectedTankItem = null;
                await Pages.Changepage(Page.Null);

            }

        }

        private async void Sharedata_PageItemselected(object? sender, Page e)
        {
            await Pages.Changepage(e);
        }



        [ObservableProperty] private string _connectionIP;
        [ObservableProperty] private string? _connectionPort;


        [ObservableProperty] private int _keytotriggers;

        [ObservableProperty] private string _statustext;
        [ObservableProperty] private bool _loadingbar = false;

        [ObservableProperty] private bool _addandclearvisib = true;





        private ObservableCollection<TankItem>? Backuplist;



        /*[ObservableProperty]*/
        private bool _connected;

        public bool Connected
        {
            get => _connected;
            set
            {
                SetProperty(ref _connected, value);

                if (value && (Pages.Pagez is null || Pages.Pagez is SettingsTankViewModel))
                {
                    Sharedata.Gotorecap = true;
                }
                else Sharedata.Gotorecap = false;

            }
        }



        private string? _search;

        public string? Search
        {
            get => _search;
            set
            {


                if (string.IsNullOrEmpty(_search) && !string.IsNullOrEmpty(value))
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
            Random rnd = new();


            //for (int i = 0; i < 150; i++)
            //{

            var newGuid = Guid.NewGuid();
            var Newitem = new TankItem
            {
                Name = newGuid.ToString()[..5] + "-TankItem",
                Id = newGuid,
                Capacity = rnd.Next(500, 1001),

            };

            Newitem.CurrentLevel = rnd.Next(Newitem.Capacity / 2, Newitem.Capacity);

            Newitem.Starring = false;

            Sharedata.Items.Add(Newitem);
            //}
            //Backuplist = Items;


        }


        

        [RelayCommand]
        private async Task Testconnection()
        {


            Loadingbar = true;

            _cTokenDisconnectfrommqtt = new CancellationTokenSource();

            if (!int.TryParse(ConnectionPort, out int port))
            {
                port = Mqttdefport;
            }

          
            await Mqtt.Checkconnection(ConnectionIP, port, _cTokenDisconnectfrommqtt.Token);
           

            Debug.WriteLine("DOONE ");

        }

        [RelayCommand]
        private async Task Disconnect()
        {

            _cTokenDisconnectfrommqtt?.Cancel();


        }

        [RelayCommand]
        private async Task GotorecapButton()
        {

            Sharedata.Gotorecap = false;
            await Pages.Changepage(Page.Recap);


        }





        [RelayCommand]
        private async Task Tests()
        {




        }









    }
}
