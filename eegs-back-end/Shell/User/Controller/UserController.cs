using eegs_back_end.Shell.User.Model;
using eegs_back_end.Shell.User.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.User.Controller
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        IUserRepository _repo;

        public UserController(IUserRepository repo)
        {
            _repo = repo;

        }
        // GET: api/<UserController>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }


        [HttpGet("get-role/{id}")]
        public object getRoleTypePerDomain(string id)
        {
            var data = _repo.GetRoleTypePerDomain(id);
            if (data == null)
                return null;
            return data;
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetByID(id);
        }

        // POST api/<UserController>
        [HttpPost]
        public IActionResult Post([FromBody] UserModel value)
        {
            if (!_repo.Insert(value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string guid, [FromBody] UserModel value)
        {
            if (!_repo.Edit(guid, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Delete Domain unsuccessful" });

            return Ok(new { response = "Domain deleted successfully" });
        }
    }
}
