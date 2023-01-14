using eegs_back_end.Economic_Enterprises.Slaughterhouse.Receiving.Model;
using eegs_back_end.Economic_Enterprises.Slaughterhouse.Receiving.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Slaughterhouse.Receiving.Controller
{
    [Route("slaughterhouse/receiving")]
    [ApiController]
    public class ReceivingController : ControllerBase
    {
        IReceivingRepository _repo;
        public ReceivingController(IReceivingRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<RentalApplication>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }

        [HttpGet("list")]
        public object GetRecList()
        {
            return _repo.GetReceivedList();
        }



        [HttpGet("dropdown")]
        public object GetDD()
        {
            return _repo.GetDropDown();
        }


    
        // GET api/<RentalApplication>/5
        [HttpGet("{id}")]
        public ReceivingModel Get(string id)
        {
            return _repo.GetByID(id);
        }
        [HttpGet("print/{id}")]
        public object GetPrint(string id)
        {
            return _repo.GetPrintByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] ReceivingModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // POST api/<BarangayOfficialController>
        [HttpPost("inspect")]
        public IActionResult Inspect([FromBody] AnimalInspection value)
        {
            if (_repo.Inspect(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] ReceivingModel value)
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
