using eegs_back_end.Shell.ChangePassword.Model;
using eegs_back_end.Shell.ChangePassword.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.ChangePassword.Controller
{
    [Route("changepassword")]
    [ApiController]
    public class ChangePasswordController : ControllerBase
    {
        IChangePasswordRepository _repo;
        public ChangePasswordController(IChangePasswordRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<ChangePasswordController>
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] ChangePasswordModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

    }
}
