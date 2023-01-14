using eegs_back_end.Admin.Barangay_Officials.Model;
using eegs_back_end.Admin.Barangay_Officials.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.Barangay_Officials.Controller
{
    [Route("admin/barangay-official")]
    [ApiController]
    public class BarangayOfficialController : ControllerBase
    {
        IBarangayOfficialRepository _repo;

        public BarangayOfficialController(IBarangayOfficialRepository repo)
        {
            _repo = repo;
        }

        // GET: api/<BarangayOfficialController>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }

        [HttpGet("employees")]
        public object GetEmployees()
        {
            return _repo.GetEmployees();
        }
        // GET api/<BarangayOfficialController>/5
        [HttpGet("{id}")]
        public BarangayOfficialModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] BarangayOfficialModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] BarangayOfficialModel value)
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

