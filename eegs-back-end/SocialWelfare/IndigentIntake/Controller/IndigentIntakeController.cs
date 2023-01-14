using eegs_back_end.Admin.IndigentIntakeSetup.Model;
using eegs_back_end.Admin.IndigentIntakeSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.IndigentIntakeSetup.Controller
{
    [Route("social_welfare/indigent_intake")]
    [ApiController]
    public class IndigentIntakeController : ControllerBase
    {
        byte[] FileAsByteArray;
        IIndigentIntakeRepository _repo;
        public IndigentIntakeController(IIndigentIntakeRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<IndigentIntakeController>
        [HttpGet]
        public List<object> Get()
        {
            return _repo.GetList();
        }

        [HttpGet("get_disability")]
        public List<object> GetListOfDisability()
        {
            return _repo.GetListOfDisability();
        }

        [HttpGet("get_person_add/{GUID}")]
        public object GetPersonAdd(string GUID)
        {
            return _repo.GetPersonAdd(GUID);
        }

        [HttpGet("indigent_intake_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<IndigentIntakeController>/5
        [HttpGet("{indigent_intake_guid}")]
        public object GetIndigentIntake(string indigent_intake_guid)
        {
            return _repo.GetIndigentIntake(indigent_intake_guid);
        }

        // POST api/<IndigentIntakeController>
        [HttpPost]
        public IActionResult Post([FromBody] IndigentIntakeModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<IndigentIntakeController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] IndigentIntakeModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<IndigentIntakeController>/5
        [HttpDelete("{indigent_intake_guid}")]
        public IActionResult Delete(string indigent_intake_guid)
        {
            var result = _repo.Delete(indigent_intake_guid);

            if (!result) return BadRequest(new { response = "Delete general intake unsuccessful" });

            return Ok(new { response = "General intake deleted successfully" });
        }
    }
}
