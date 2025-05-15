using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        if (Guid.TryParse(value.ToString(), out _))
        {
            return true;
        }

        return false;

        // only check string length if empty strings are not allowed
        //  return AllowGuid || value is not string stringValue || !string.IsNullOrWhiteSpace(stringValue);
    }


}