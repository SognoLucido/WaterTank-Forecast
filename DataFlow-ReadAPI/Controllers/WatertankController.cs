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
            "All null property values will be IGNORED (in the response body)")]
        [HttpGet]
        [Route("info")]
        [ProducesResponseType(typeof(ResponseWrapper), StatusCodes.Status200OK) ]
        public async Task<IActionResult> GetTankValor(
        [Description("array GUID[] : At least one Id is required . es. CAA73977-B67A-426D-8987-3EBCE846A452")]
        [Required]
        [FromQuery]
        Guid[] Tankid,
        [Description("the date-time notation as defined by RFC 3339, section 5.6, for example, 2017-07-21T17:32:28Z or 2017-07-21 .The end date is already fixed to be inclusive unless the hour, minute, second are specified")]
        DateTime? dateRange1,
        [Description("the date-time notation as defined by RFC 3339, section 5.6, for example, 2017-07-21T17:32:28Z or 2017-07-21 .The end date is already fixed to be inclusive unless the hour, minute, second are specified")]
        DateTime? dateRange2,
        bool ClientId = false,
        bool ZoneCode = false,
        bool TotalCapacity = false)
        {


            //Guid[] datatest = 
            //{
            //    new("05bd994b-5109-438b-92e7-6a14829718d4") , 
            //    new("39bf125b-2d2f-4f00-aae8-b5d598f89dbb") , 
            //    new("3ff56eb2-4227-4c2e-ae7f-89bffd54ac23")
            //};

            if (dateRange1 is not null && dateRange2 is not null)
            {
                //magic swap
                if (dateRange1 > dateRange2) (dateRange1, dateRange2) = (dateRange2, dateRange1);
     
                var returnz = await dbcall.GetinfoItem(Tankid, ClientId, ZoneCode, TotalCapacity, (DateTime)dateRange1, (DateTime)dateRange2);
                return returnz is not null ? Ok(returnz) : NotFound();
            }
            else 
            {
                var returz = await dbcall.GetinfoItem(Tankid, ClientId, ZoneCode, TotalCapacity);

                return returz is not null ? Ok(returz) : NotFound();
            }
           // return Ok();
        }

        //TODO
        // GET: api/<WatertankController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {

            // await dbcall.Fetchdata(null,0);

            return Ok("TODO");

            //return Ok(await dbcall.Fetchdata(null, 0));
        }

        //// GET api/<WatertankController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<WatertankController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<WatertankController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<WatertankController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
