using eegs_back_end.Shell.Form.Model;
using eegs_back_end.Shell.Form.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.Form.Controller
{
    [Route("form")]
    [ApiController]
    public class FormController : ControllerBase
    {
        IFormRepository _repo;
        public FormController(IFormRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<FormController>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }

        // GET api/<FormController>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return  _repo.GetByID(id);
        }

        // POST api/<FormController>
        [HttpPost]
        public IActionResult Post([FromBody] FormModel value)
        {
            var result = _repo.Insert(value);

            if (!result) return BadRequest("Add Form unsuccessful");

            return Ok(new { response = "Form added successfully" });
        }

        // PUT api/<FormController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] FormModel value)
        {
            var result = _repo.Edit(id, value);
            if (!result) return BadRequest(new { response = "Edit Form unsuccessful" });
            return Ok(new { response = "Form addded successfully" });
        }

        // DELETE api/<FormController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        [HttpGet("folders/{id}")]
        public object getFolders(string id)
        {
            return _repo.GetDomainFolders(id);
        }
        [HttpGet("filters")]
        public object getFilter(string id)
        {
            return _repo.GetFilterOptions();
        }
        [HttpGet("action-type")]
        public object getActions(string id)
        {
            return _repo.GetActionTypes();
        }

    }
}
