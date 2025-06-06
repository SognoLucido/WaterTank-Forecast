using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace WaterTankMockTool_MQTT.Services;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false)]
public sealed class GuidAttribute : ValidationAttribute
{


    public override bool IsValid(object? value)
    {
        if (value is null) return true;
        if (Guid.TryParse(value?.ToString(), out _))
        {
            return true;
        }

        return false;

    }


}



[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
      AllowMultiple = false)]
public sealed class LessThanAttribute : ValidationAttribute
{

     string Bnumber { get; }
     


    /// <param name="b">The number that the first parameter will be compared to.</param>
    public LessThanAttribute(string b)   
    {
       Bnumber = b;
    }



    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {


        if (value == null)
        {
            return new ValidationResult(null);
        }

        var BValue = validationContext.ObjectInstance.GetType().GetProperty(Bnumber)?.GetValue(validationContext.ObjectInstance) ?? null;



        if (int.TryParse(value.ToString(), out var MinorNumber))
        {
            if (int.TryParse(BValue?.ToString(), out var BNumber))
            {
                return MinorNumber <= BNumber ? ValidationResult.Success : new ValidationResult(null);
            }
        }

        return new ValidationResult(null);

    }

    //public override bool IsValid(object? value )
    //{


    //    if (value == null)
    //    {
    //        return true;
    //    }

    //    if(int.TryParse(value.ToString(), out var MinorNumber))
    //    {
    //        if(int.TryParse(Bnumber, out var BNumber))
    //        {
    //            return MinorNumber <= BNumber ;
    //        }
    //    }

    //    return false;
    //}


}