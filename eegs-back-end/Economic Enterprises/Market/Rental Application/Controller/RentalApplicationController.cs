using eegs_back_end.Economic_Enterprises.Market.Rental_Application.Model;
using eegs_back_end.Economic_Enterprises.Market.Rental_Application.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Market.Rental_Application.Controller
{
    [Route("market/rental-application")]
    [ApiController]
    public class RentalApplicationController : ControllerBase
    {
        IRentalApplicationRepository _repo;
        public RentalApplicationController(IRentalApplicationRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<RentalApplication>
        [HttpGet]
        public object Get([FromQuery] int status_id, [FromQuery] string type)
        {
            return _repo.GetList(status_id, type);
        }


        [HttpGet("get-types")]
        public object GetDD()
        {
            return _repo.GetDropDown();
        }


        [HttpGet("requirements/{type}")]
        public object GetReqs(int type)
        {
            return _repo.GetRequirements(type);
        }

        // GET api/<RentalApplication>/5
        [HttpGet("{id}")]
        public RentalApplicationModel Get(string id)
        {
            return _repo.GetByID(id);
        }

        [HttpGet("transient")]
        public object GetTransient([FromQuery] string dte)
        {
            return _repo.GetTransientList(dte);
        }

        [HttpGet("stall")]
        public object GetStall([FromQuery] DateTime dte)
        {
            return _repo.GetStallList(dte);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] RentalApplicationModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] RentalApplicationModel value)
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
