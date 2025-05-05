using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DataFlow_ReadAPI.Models;
using DataFlow_ReadAPI.Services.DBFetching;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataFlow_ReadAPI.Controllers
{
    [Route("api/tank")]
    [ApiController]
    public class WatertankController(IDbFetch _dbcall) : ControllerBase
    {

        private readonly IDbFetch dbcall = _dbcall;


        [EndpointDescription("" +
            "This endpoint uses `tankid[]` as a query parameter (with a limitation on URL length)." +
            "By default, it returns the MOST RECENT DATA for the provided `Tankid`." +
            "If a `dateRange1/2` query parameter is used, it will return a date range that matches the WHERE clause in the database query." +
            "All null response values will be ignored (in the response body)")]
        [HttpGet]
        [Route("info")]
        [ProducesResponseType(typeof(ResponseWrapper), StatusCodes.Status200OK) ]
        public async Task<IActionResult> GetTankValor(
        [Description("array GUID[] : At least one Id is required . es. CAA73977-B67A-426D-8987-3EBCE846A452")]
        [Required]
        [FromQuery]
        Guid[] tankid,
        [Description("the date-time notation as defined by RFC 3339, section 5.6, for example, 2017-07-21T17:32:28Z or 2017-07-21 .The end date is already fixed to be inclusive unless the hour, minute, second are specified")]
        DateTime? dateRange1,
        [Description("the date-time notation as defined by RFC 3339, section 5.6, for example, 2017-07-21T17:32:28Z or 2017-07-21 .The end date is already fixed to be inclusive unless the hour, minute, second are specified")]
        DateTime? dateRange2,
        bool clientid = false,
        bool zonecode = false,
        bool totalcapacity = false)
        {

            if (dateRange1 is not null && dateRange2 is not null)
            {
                //magic swap
                if (dateRange1 > dateRange2) (dateRange1, dateRange2) = (dateRange2, dateRange1);
     
                var returnz = await dbcall.GetinfoItem(tankid, clientid, zonecode, totalcapacity, (DateTime)dateRange1, (DateTime)dateRange2);
                return returnz is not null ? Ok(returnz) : NotFound();
            }
            else 
            {
                var returz = await dbcall.GetinfoItem(tankid, clientid, zonecode, totalcapacity);

                return returz is not null ? Ok(returz) : NotFound();
            }
      
        }





        [EndpointDescription("" +
            "'GUID[] tankid' and (optional)`dateRange1/2` as body" +
            "By default, it returns the MOST RECENT DATA for the provided `Tankid`." +
            "If both`dateRange1/2` are specified, it returns data within the matching date range based on the WHERE clause in the database query." +
            "All null response values will be ignored (in the response body)")]
        [HttpPost]
        [Route("info")]
        [ProducesResponseType(typeof(ResponseWrapper), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTankValorPost(
        [FromBody] DbinfoPostDTO data,
        bool clientid = false,
        bool zonecode = false,
        bool totalcapacity = false)
        {


            if (data.dateRange1 is not null && data.dateRange2 is not null)
            {
                //magic swap
                if (data.dateRange1 > data.dateRange2) (data.dateRange1, data.dateRange2) = (data.dateRange2, data.dateRange1);

                var returnz = await dbcall.GetinfoItem(data.Tankid, clientid, zonecode, totalcapacity, (DateTime)data.dateRange1, (DateTime)data.dateRange2);
                return returnz is not null ? Ok(returnz) : NotFound();
            }
            else
            {
                var returz = await dbcall.GetinfoItem(data.Tankid, clientid, zonecode, totalcapacity);

                return returz is not null ? Ok(returz) : NotFound();
            }


        }



        [EndpointDescription("Calculate how many days will be required to empty the item's resource. All possible parameter combinations are allowed, except when all are null and range_days itself " +
            "'tankids' : multiple items , 'client_id' : single , 'zone_code' : single . 'Guid[] Tankids' as a query parameter ")]
        [HttpGet]
        [Route("forecast")]
        [ProducesResponseType(typeof(DBreturnDataDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetForecastdata(
            [FromQuery]
            [Description("Guid example in 'scalar form' :123e4567e89b12d3a456426614174000,123e4567-e89b-12d3-a456-426614174001 converted in query tankids=123e4567e89b12d3a456426614174000%2C123e4567-e89b-12d3-a456-426614174001 ")]
            Guid[]? tankids,
            Guid? client_id,
            [MaxLength(10)]
            string? zone_code,
            [Range(3,360)]
            int range_days = 30
            )
        {


            if (client_id is null && zone_code is null && (tankids is null || tankids.Length == 0))
                return BadRequest();


            var dbdata = await dbcall.Forecast(tankids, range_days, client_id, zone_code);

            return dbdata is null ? NotFound() : Ok(dbdata);
   
        }



        [EndpointDescription("Forecast POST endpoint, similar to the GET endpoint, using Guid[]? Tankid in the POST body")]
        [HttpPost]
        [Route("forecast")]
        [ProducesResponseType(typeof(DBreturnDataDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> PostForecastdata(    
        DbforecastbodyParam? bodydata,
        Guid? client_id,
        [MaxLength(10)]
        string? zone_code,
        [Range(3,360)]
        int range_days = 30
        )
        {


            if (client_id is null && zone_code is null && (bodydata?.Tankids is null || bodydata.Tankids.Length == 0))
                return BadRequest();


            var dbdata = await dbcall.Forecast(bodydata?.Tankids, range_days, client_id, zone_code);

            return dbdata is null ? NotFound() : Ok(dbdata);

        }






    }
}
