using eegs_back_end.Admin.Department.Model;
using eegs_back_end.Admin.Department.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.Department.Controller
{
    [Route("admin/department")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        IDepartmentRepository _repo;

        public DepartmentController(IDepartmentRepository repo)
        {
            _repo = repo;
        }

        // GET: api/<DepartmentController>
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

        [HttpGet("prefix")]
        public object GetPrefix([FromQuery] string id)
        {
            return new {prefix = _repo.GetPrefix(id) };
        }
        // GET api/<DepartmentController>/5
        [HttpGet("{id}")]
        public DepartmentModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<DepartmentController>
        [HttpPost]
        public IActionResult Post([FromBody] DepartmentModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<DepartmentController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] DepartmentModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<DepartmentController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {

            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }
    }
}
