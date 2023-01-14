using eegs_back_end.Economic_Enterprises.Slaughterhouse.ScheduleOfFees.Model;
using eegs_back_end.Economic_Enterprises.Slaughterhouse.ScheduleOfFees.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.ScheduleOfFees.Controller
{
    [Route("slaughterhouse/schedule_of_fees")]
    [ApiController]
    public class ScheduleOfFeesController : ControllerBase
    {
        // GET: api/<ScheduleOfFeesController>
        IScheduleOfFeesRepository _repo;
        public ScheduleOfFeesController(IScheduleOfFeesRepository repo)
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
        public IActionResult Post([FromBody] ScheduleOfFeesModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] ScheduleOfFeesModel value)
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
