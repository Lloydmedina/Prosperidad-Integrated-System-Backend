using eegs_back_end.Admin.BusinessSetup.Model;
using eegs_back_end.Admin.BusinessSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.BusinessSetup.Contoller
{
    [Route("admin/business")]
    [ApiController]
    public class BusinessController : ControllerBase
    {
        IBusinessRepository _repo;
        public BusinessController(IBusinessRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<BusinessController>
        [HttpGet]
        public object Get([FromQuery] int status_id, int reg_status)
        {
            return _repo.GetList(status_id, reg_status);
        }

        [HttpGet("get_business_entity")]
        public object GetBusinessEntity()
        {
            return _repo.GetBusinessEntity();
        }

        [HttpGet("history")]
        public object GetHistory([FromQuery] string ID)
        {
            return _repo.GetHistory(ID);
        }

        // GET api/<BusinessController>/5
        [HttpGet("{id}")]
        public BusinessModel Get(string id)
        {
            return _repo.GetByID(id);
        }

        // POST api/<BusinessController>
        [HttpPost]
        public IActionResult Post([FromBody] BusinessModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BusinessController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] BusinessModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<BusinessController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {

            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }
    }
}
