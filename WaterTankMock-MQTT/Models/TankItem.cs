using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace WaterTankMock_MQTT.Models;

public partial class TankItem : ObservableObject
{


    public TankItem() 
    {
        Triggers =
            [
             new TriggerItem("Trigger1",false),
            new TriggerItem("Trigger2",false),
            new TriggerItem("Trigger3",false),
            new TriggerItem("Trigger4",false),
            new TriggerItem("Trigger5",true),
            new TriggerItem("Trigger6",false),
            new TriggerItem("Trigger7",false),
            new TriggerItem("Trigger8",false),
            new TriggerItem("Trigger9",false),
            new TriggerItem("Trigger10",false)
            ];

    
        Triggerfiltered = [];

    }

    //public string Name { get; set; }




    private string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (string.IsNullOrEmpty(value)) return;
            SetProperty(ref _name, value);
        }
    }


    //[ObservableProperty]
    //private bool _starred;

    [ObservableProperty] private string _stariconcolor;
    [ObservableProperty] private string _stariconname;


    private bool _starring;
    public bool Starring
    {
        get
        {
            TriggerIconstar(_starring);
            return _starring;
        }
        set
        {
            SetProperty(ref _starring, value);

            TriggerIconstar(value);

        }
    }

    public string? Description { get; set; }

    public Guid Id { get; set; }

    public int Capacity { get; set; }

    public int CurrentLevel { get; set; }





    private void TriggerIconstar(bool status)
    {

        if (status)
        {
            Stariconcolor = "Yellow";
            Stariconname = "Star";
        }
        else
        {
            Stariconcolor = "";
            Stariconname = "StarOutline";
        }


    }

    /// <summary>
    /// (active-not,)
    /// </summary>
    //public (bool, int, int)[] Triggers { get; set; } =
    //[
    //        new(false,0,0),
    //        new(false,0,0),
    //        new(false,0,0),
    //        new(false,0,0),
    //        new(true,0,0),
    //        new(false,0,0),
    //        new(false,0,0),
    //        new(false,0,0),
    //        new(false,0,0)
    //];

    //[ObservableProperty]
    //public bool[] _triggerOn  = [false,false,false,false,true,false,false,false,false,false];


    //[ObservableProperty]
    //private ObservableCollection<bool>? _triggerOn = [false, false, false, false, true, false, false, false, false, false];

    //public ObservableCollection<bool>? TriggerOn
    //{
    //    get => _triggerOn;

    //    set
    //    {
    //        _triggerOn = value;




    //        //OnPropertyChanged(nameof(Triggernames));
    //    }
    //}


    //[ObservableProperty]
    //private List<string> _triggersnames;





    //private ObservableCollection<TriggerItem> _triggers;

    [ObservableProperty]
    public ObservableCollection<TriggerItem> _triggers;


    [ObservableProperty]
    public ObservableCollection<TriggerItem> _triggerfiltered;


    


    //[ObservableProperty]
    //private Dictionary<int, string> _triggercombobox;


    //private List<TriggerItem> _yolo;

    //public List<TriggerItem> Yolo
    //{
    //    get 
    //    {
    //        return Triggers.Where(x=> x.Active == true).ToList();
    //    }
       
    //}



    //public List<String>? Triggernames 
    //{
    //    get 
    //    {
    //        List<String> list = [];

    //        for(int i = 0; i < 10; i++)
    //        {
    //            if (TriggerOn[i]) list.Add($"Trigger{i+1}");
    //        }

    //        return list;
    //    }


    //}


    [ObservableProperty]
    private int _activetriggers = 1;

}



public class TriggerItem(string name ,bool active)
{
    public bool Active { get; set; } = active;

    
    public int Rangemin { get; set; } = 0;

   
    public int Rangemax { get; set; } = 0;

    public string Name { get; set; } = name;
}