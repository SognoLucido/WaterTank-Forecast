using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WaterTankMock_MQTT.Models;
using WaterTankMockTool_MQTT.Services;


namespace WaterTankMock_MQTT.ViewModels
{
    public partial class SettingsTankViewModel : ViewModelBase
    {

        public Sharedata Sharedata { get; }

        public SettingsTankViewModel() { }

        private TriggerItem? _triggeritemselect;
        private bool _settings;


        //[ObservableProperty] private string? _settingerror;
        [ObservableProperty] private string _togglesettingbuttonname;
        [ObservableProperty] private bool _rangevisible;
        [ObservableProperty] private string? _test = "ACTIVE";
         
        private string? _number2;
        private string? _number1;

        private string? _tempname;

         private string? _tempCap;
         private string? _tempCurrlvl;
         private string? _tempGuidid;
        public SettingsTankViewModel(Sharedata sharedata)
        {
            Sharedata = sharedata;
            Togglesettingbuttonname = "Settings";
            Tankinfo_init();
            Number1 = "0";
            Number2 = "0";
        }


        [Required(ErrorMessage = "*Required")]
        [StringLength(50, ErrorMessage = "Name cannot be longer than 50 characters")]
        public string? Tempname
        {
            get => _tempname;
            set
            {
                SetProperty(ref _tempname, value);
                ValidateProperty(value, nameof(Tempname));
            }
        }

        [Required(ErrorMessage = "*Required")]
        [RegularExpression(@"^[\d+]{1,20}$", ErrorMessage = "invalid Capacity")] 
        public string? TempCap
        {
            get => _tempCap;
            set
            {
                SetProperty(ref _tempCap, value);
                ValidateProperty(value,nameof(TempCap));
                ValidateProperty(TempCurrlvl, nameof(TempCurrlvl));
            }
        }

        [Required(ErrorMessage ="*Required")]
        [LessThan(nameof(TempCap), ErrorMessage = "Invalid property must be less than or equal to Capacity")]
        public string? TempCurrlvl
        {
            get => _tempCurrlvl;
            set
            {
                SetProperty(ref _tempCurrlvl, value);
                ValidateProperty(value, nameof(TempCurrlvl));
            }
        }

        [Required(ErrorMessage = "*Required")]
        [Guid(ErrorMessage = "Invalid Guid")]
        public string? TempGuidid
        {
            get => _tempGuidid;
            set
            {
                SetProperty(ref _tempGuidid, value);
                ValidateProperty(value, nameof(TempGuidid));
            }
        }




        /// <summary>
        /// ///////////////////////////////////////////
        /// </summary>
        ///
        [Required]
        [RegularExpression(@"^[\d+]{1,5}$", ErrorMessage = "invalid Capacity")]
        public string? Number1
        {
            get => _number1;
            set
            {
                SetProperty(ref _number1, value); 
                ValidateProperty(value, nameof(Number1));
            }
        }

        [Required]
        [RegularExpression(@"^[\d+]{1,5}$", ErrorMessage = "invalid Capacity")]
        public string? Number2
        {
            get => _number2;
            set
            {
                SetProperty(ref _number2, value);
                ValidateProperty(value, nameof(Number2));
            }
        }


        private void Tankinfo_init()
        {
            Tempname = Sharedata?.SelectedTankItem?.Name ?? string.Empty;
            TempCap = Sharedata?.SelectedTankItem?.Capacity.ToString() ?? string.Empty;
            TempCurrlvl = Sharedata?.SelectedTankItem?.CurrentLevel.ToString() ?? string.Empty;
            TempGuidid = Sharedata?.SelectedTankItem?.Id.ToString() ?? string.Empty;
        }

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

                    Number1 = value.Rangemin.ToString();
                    Number2 = value.Rangemax.ToString();

                }
                else Rangevisible = false;


            }
        }


        public bool Settings
        {
            get => _settings;
            set
            {
                SetProperty(ref _settings, value);
                if (value)
                {
                   ResetTriggerListTOnull();


                    Sharedata.SelectedTankItem.Triggerfiltered = new ObservableCollection<TriggerItem>(Sharedata.SelectedTankItem.Triggers.Where(x => x.Active == true).ToArray());


                    Sharedata.SelectedTankItem.Activetriggers = Sharedata.SelectedTankItem.Triggers.Count(x => x.Active);
                    Togglesettingbuttonname = "Triggers";
                }
                else Togglesettingbuttonname = "Settings";
            }
        }




        //EDIT TO TEST
        [RelayCommand]
        private async Task Saverange()
        {


           // await ResetRange();

            Tankinfo_init();
            if (HasErrors) return;

            if (!int.TryParse(_number1, out int n1) || !int.TryParse(_number2, out int n2)) return;
            else if (n1 < 0 || n2 < 0) return;
            else
            {
                if (Triggeritemselect is not null)
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

            await ResetTriggerListTOnull();

        }

        private async Task ResetTriggerListTOnull() =>Triggeritemselect = null;
       
        private async Task ResetRange()
        {
            if (Sharedata.SelectedTankItem is not null && Triggeritemselect is not null)
            {
                //Default
                Number1 = "1";
                Number2 = "10";

                //Triggeritemselect.Rangemin = 1;
                //Triggeritemselect.Rangemax = 10;

                //ValidateProperty("1", nameof(Number1));
                //ValidateProperty("10", nameof(Number2));
            }
        }


        private async Task SavetoMainshareDATA()
        {

            if (Sharedata.SelectedTankItem is not null) 
            {

                Sharedata.SelectedTankItem.Name = Tempname;
                Sharedata.SelectedTankItem.Capacity = int.Parse(TempCap!);
                Sharedata.SelectedTankItem.CurrentLevel = int.Parse(TempCurrlvl!);
                Sharedata.SelectedTankItem.Id = new Guid(TempGuidid!);
            }



        }

       
        //EDIT TEST
        [RelayCommand]
        private async Task Savedata()
        {


          


            if(Triggeritemselect is not null) await ResetRange();
             Triggeritemselect = null;

            ValidateAllProperties();

            if (HasErrors) return;



            await SavetoMainshareDATA();
            Sharedata.SelectedTankItem = null;

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
