using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using eegs_back_end.Helpers;
using eegs_back_end.HR.Position_Setup.Model;
using System.Linq;
using System.Threading.Tasks;
using eegs_back_end.HR.Position_Setup.Repository;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.HR.Position_Setup.Controller
{
    [Route("human-resource/position")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        IPositionRepository _repo;

        public PositionController(IPositionRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<PositionController>
        [HttpGet]
        public object GetList()
        {
            return _repo.GetList();
        }

        [HttpGet("dropdown")]
        public object getDropdown()
        {
            return _repo.GetDropDown();
        }


        [HttpGet("{id}")]
        public PositionModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] PositionModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] PositionModel value)
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
