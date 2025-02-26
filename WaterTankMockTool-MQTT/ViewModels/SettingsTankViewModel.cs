using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WaterTankMock_MQTT.Models;


namespace WaterTankMock_MQTT.ViewModels
{
    public partial class SettingsTankViewModel : ViewModelBase
    {

        public Sharedata Sharedata { get; }

        public SettingsTankViewModel() { }

        public SettingsTankViewModel(Sharedata sharedata) 
        {
            Sharedata = sharedata;
            Togglesettingbuttonname = "Settings";
        }

        [ObservableProperty] private string? _settingerror;
        [ObservableProperty] private string _togglesettingbuttonname ;
        [ObservableProperty] private bool _rangevisible;
        [ObservableProperty] private string? _test = "ACTIVE";
        //[ObservableProperty]
        private TriggerItem? _triggeritemselect;

        public TriggerItem? Triggeritemselect
        {
            get
            {


                return _triggeritemselect;
            }
            set
            {
                SetProperty(ref _triggeritemselect, value);


                if (value is not null)
                {
                    Rangevisible = true;
                    

                    //////


                }
                else Rangevisible = false;


            }
        }

        private bool _settings;
        public bool Settings
        {
            get => _settings;
            set
            {
                SetProperty(ref _settings, value);
                if (value)
                {
                    Cleandata();
             

                    Sharedata.SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(Sharedata.SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray());

                  
                    Sharedata.SelectedTankItem.Activetriggers = Sharedata.SelectedTankItem.Triggers.Count(x => x.Active);
                    Togglesettingbuttonname = "Triggers";
                }
                else Togglesettingbuttonname = "Settings";
            }
        }


        [ObservableProperty]
        private string? _number1 ;


        [ObservableProperty]
        private string? _number2 ;



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

        private async Task Cleandata()
        {
            Number1 = "";
            Number2 = "";
            Triggeritemselect = null;
            Settingerror = string.Empty;



        }


        [RelayCommand]
        private async Task Star()
        {
            if (/*_itemSelected &&*/ Sharedata.SelectedTankItem is not null)
            {
                Sharedata.SelectedTankItem.Starring = !Sharedata.SelectedTankItem.Starring;


                Sharedata.Items = new ObservableCollection<TankItem>(Sharedata.Items.OrderByDescending(item => item.Starring));

            }

        }

    }
}
