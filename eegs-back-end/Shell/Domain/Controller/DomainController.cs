using eegs_back_end.Shell.Domain.Model;
using eegs_back_end.Shell.Domain.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.Domain.Controller
{
    [Route("domain")]
    [ApiController]
    public class DomainController : ControllerBase
    {
        IDomainRepository _repo;
        public DomainController(IDomainRepository repo)
        {
            _repo = repo;
        }

        // GET: api/<DomainController>
        [HttpGet]
        public List<object> Get()
        {
            var data = _repo.GetList();
            if (data == null)
                return null;
            return data;
        }

        // GET api/<DomainController>/5
        [HttpGet("{id}")]
        public DomainModel Get(string id)
        {
            return _repo.GetByID(id);
        }

        // POST api/<DomainController>
        [HttpPost]
        public IActionResult Post([FromBody] DomainModel value)
        {

            var result = _repo.Insert(value);

            if (!result) return BadRequest(new { response = "Add Domain unsuccessful" });

            return Ok(new { response = "Domain addded successfully" });
        }

        // PUT api/<DomainController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] DomainModel value)
        {

            var result = _repo.Edit(id, value);

            if (!result) return BadRequest(new { response = "Edit Domain unsuccessful" });

            return Ok(new { response = "Domain update successful" });
        }

        // DELETE api/<DomainController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Delete Domain unsuccessful" });

            return Ok(new { response = "Domain deleted successfully" });
        }

        [HttpGet("project")]
        public List<object> GetProject()
        {
            var data = _repo.GetProject();
            if (data == null)
                return null;
            return data;
        }
    }
}
