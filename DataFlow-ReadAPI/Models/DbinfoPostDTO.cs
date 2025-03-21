using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataFlow_ReadAPI.Models
{
    public class DbinfoPostDTO
    {
        [Required]
        [Description("array GUID[] : At least one Id is required . es. CAA73977-B67A-426D-8987-3EBCE846A452")]
        public Guid[] Tankid { get; set; }

        [Description("the date-time notation as defined by RFC 3339, section 5.6, for example, 2017-07-21T17:32:28Z or 2017-07-21 .The end date is already fixed to be inclusive unless the hour, minute, second are specified")]
        public DateTime? dateRange1 { get; set; }

        [Description("the date-time notation as defined by RFC 3339, section 5.6, for example, 2017-07-21T17:32:28Z or 2017-07-21 .The end date is already fixed to be inclusive unless the hour, minute, second are specified")]
        public DateTime? dateRange2 { get; set; }

    }
}
