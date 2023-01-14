using eegs_back_end.Accounting.AccountTitle.Model;
using eegs_back_end.Accounting.AccountTitle.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Accounting.AccountTitle.Controller
{
    [Route("accounting/account-title")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        IAccountRepository _repo;
        public AccountController(IAccountRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<AccountController>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }

        [HttpGet("dropdown")]
        public object getDropdown()
        {
            return _repo.GetDropDown();
        }

        [HttpGet("generate-code")]
        public object GenerateCode([FromQuery] int parent_id)
        {
            return _repo.GenerateAccountCode(parent_id);
        }

        [HttpGet("{id}")]
        public AccountModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] AccountModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] AccountModel value)
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
