using DataFlow_ReadAPI.Services.DBFetching;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DataFlow_ReadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WatertankController : ControllerBase
    {

        private readonly IDbFetch dbcall;
        public WatertankController(IDbFetch _dbcall) { dbcall = _dbcall; }    





        // GET: api/<WatertankController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {

           // await dbcall.Fetchdata(null,0);

            return Ok(await dbcall.Fetchdata(null, 0));
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
