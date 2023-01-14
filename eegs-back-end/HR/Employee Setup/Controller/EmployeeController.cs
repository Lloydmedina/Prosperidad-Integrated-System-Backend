using eegs_back_end.HR.Employee_Setup.Model;
using eegs_back_end.HR.Employee_Setup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.HR.Employee_Setup.Controller
{
    [Route("human-resource/employee")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        IEmployeeRepository _repo;

        public EmployeeController(IEmployeeRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public object GetList()
        {
            return _repo.GetList();
        }

        [HttpGet("get_active_list")]
        public object GetActiveList()
        {
            return _repo.GetActiveList();
        }

        [HttpGet("dropdown")]
        public object getDropdown()
        {
            return _repo.GetDropDown();
        }


        [HttpGet("{id}")]
        public EmployeeModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] EmployeeModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] EmployeeModel value)
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
