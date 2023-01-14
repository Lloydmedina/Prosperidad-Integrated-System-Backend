using eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Model;
using eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Utility_Setup.Controller
{
    [Route("market/utilities-setup")]
    [ApiController]
    public class UtilityController : ControllerBase
    {
        IUtilityRepository _repo;
        public UtilityController(IUtilityRepository repo)
        {
            _repo = repo;
        }

        [HttpGet("water")]
        public object GetListW()
        {
            return _repo.GetList("Water");
        }

        [HttpGet("water/get_active_list")]
        public object GetActiveListW()
        {
            return _repo.GetActiveList("Water");
        }

        [HttpGet("electricity")]
        public object GetListE()
        {
            return _repo.GetList("Electricity");
        }

        [HttpGet("electricity/get_active_list")]
        public object GetActiveListE()
        {
            return _repo.GetActiveList("Electricity");
        }

        [HttpGet("{id}")]
        public UtilityModel Get(string id)
        {
            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] UtilityModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] UtilityModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<BarangayOfficialController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id, [FromQuery] string remarks)
        {

            var result = _repo.Delete(id, remarks);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }

        //[HttpGet("print-list")]
        //public object GetPrintList()
        //{
        //    return _repo.GetPrintList();
        //}
    }
}
