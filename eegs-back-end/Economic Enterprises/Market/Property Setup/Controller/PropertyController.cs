using eegs_back_end.Economic_Enterprises.Market.Property_Setup.Model;
using eegs_back_end.Economic_Enterprises.Market.Property_Setup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Market.Property_Setup.Controller
{
    [Route("market/property-setup")]
    [ApiController]
    public class PropertyController : ControllerBase
    {

        IPropertyRepository _repo;

        public PropertyController(IPropertyRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<FCController>
        [HttpGet]
        public object GetList([FromQuery] int status_id)
        {
            return _repo.GetList(status_id);
        }

        [HttpGet("dropdown")]
        public object getDropdown()
        {
            return _repo.GetDropDown();
        }


        [HttpGet("{id}")]
        public PropertyModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] PropertyModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] PropertyModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<BarangayOfficialController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id, string remarks)
        {

            var result = _repo.Delete(id, remarks);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }
    }
}
