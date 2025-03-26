using System.ComponentModel;

namespace DataFlow_ReadAPI.Models
{
    public class DbforecastbodyParam
    {
       [Description("'nullable' .Body es : {\"tankids\": [\"123e4567-e89b-12d3-a456-426614174000\",\"333e4567-e89b-12d3-a456-426614174000\" ] }")]
        public Guid[]? Tankids { get; set; }
    }
}
