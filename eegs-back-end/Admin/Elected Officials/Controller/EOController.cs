using eegs_back_end.Admin.Elected_Officials.Model;
using eegs_back_end.Admin.Elected_Officials.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.Elected_Officials.Controller
{
    [Route("admin/elected-officials")]
    [ApiController]
    public class EOController : ControllerBase
    {
        IEORepository _repo;
        public EOController(IEORepository repo)
        {
            _repo = repo;
        }
        // GET: api/<EOController
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

        // GET api/<EOController>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetByID(id);
        }

        [HttpGet("print/{id}")]
        public object GetPrint(string id)
        {
            return _repo.GetPrintable(id);
        }


        // POST api/<EOController>
        [HttpPost]
        public IActionResult Post([FromBody] EOModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<EOController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] EOModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<EOController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }
    }
}
