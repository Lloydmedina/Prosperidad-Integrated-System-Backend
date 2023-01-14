using eegs_back_end.Admin.Signatory.Model;
using eegs_back_end.Admin.Signatory.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.Signatory.Controller
{
    [Route("admin/signatory")]
    [ApiController]
    public class SignatoryController : ControllerBase
    {
        ISignatoryRepository _repo;
        public SignatoryController(ISignatoryRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<SignatoryController>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }
        [HttpGet("dropdown")]
        public object GetDropdown()
        {
            return _repo.GetDropdown();
        }

        [HttpGet("forms")]
        public object GetDropdown(string id)
        {
            return _repo.GetDropdown();
        }


        [HttpGet("department")]
        public object GetDept()
        {
            return _repo.GetDepartment();
        }

        // GET api/<SignatoryController>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetByID(id);
        }

        // POST api/<SignatoryController>
        [HttpPost]
        public IActionResult Post([FromBody] SignatoryModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new {result = "Unsuccessful"});
        }

        // PUT api/<SignatoryController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] SignatoryModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<SignatoryController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }
    }
}
