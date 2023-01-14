using eegs_back_end.Shell.RoleType.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using eegs_back_end.Shell.RoleType.Model;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.RoleType.Controller
{
    [Route("roletype")]
    [ApiController]
    public class RoleTypeController : ControllerBase
    {
        IRoleTypeRepository _repo;
        public RoleTypeController(IRoleTypeRepository repo)
        {
            _repo = repo;
        }


        [HttpGet("activity")]
        public object GetActivity(string domain_guid)
        {
            return _repo.GetActivityType(domain_guid);
        }


        // GET: api/<RoleTypeController>
        [HttpGet]
        public List<object> Get()
        {
            return _repo.GetList();
        }

        // GET api/<RoleTypeController>/5
        [HttpGet("{id}")]
        public RoleTypeModel Get(string id)
        {
            return _repo.GetByID2(id);
        }

        // POST api/<RoleTypeController>
        [HttpPost]
        public IActionResult Insert([FromBody] RoleTypeModel model)
        {
            var res = _repo.Insert(model);

            if (!res) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<RoleTypeController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] RoleTypeModel value)
        {

            var result = _repo.Edit(id, value);

            if (!result) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }


        // DELETE api/<DomainController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Delete RoleType unsuccessful" });

            return Ok(new { response = "RoleType deleted successfully" });
        }
    }
}
