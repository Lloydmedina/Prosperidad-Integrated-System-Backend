using eegs_back_end.Economic_Enterprises.Slaughterhouse.Billing.Model;
using eegs_back_end.Economic_Enterprises.Slaughterhouse.Billing.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.Billing.Controller
{
    [Route("slaughterhouse/billing")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        // GET: api/<BillingController>
        IBillingRepository _repo;
        public BillingController(IBillingRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<RentalApplication>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }

        [HttpGet("print")]
        public object GetPrint([FromQuery] int month)
        {
            return _repo.GetPrint(month);
        }

        // GET api/<RentalApplication>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetByID(id);
        }
        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] BillingModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] BillingModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<BarangayOfficialController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {

            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }
    }
}
