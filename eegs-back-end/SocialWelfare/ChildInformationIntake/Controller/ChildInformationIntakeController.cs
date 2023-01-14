using eegs_back_end.Admin.ChildInformationIntakeSetup.Model;
using eegs_back_end.Admin.ChildInformationIntakeSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.ChildInformationIntakeSetup.Controller
{
    [Route("social_welfare/child_intake")]
    [ApiController]
    public class ChildInformationIntakeController : ControllerBase
    {
        byte[] FileAsByteArray;
        IChildInformationIntakeRepository _repo;
        public ChildInformationIntakeController(IChildInformationIntakeRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<ChildInformationIntakeController>
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

        [HttpGet("get_educational_type")]
        public List<object> GetEducationalType()
        {
            return _repo.GetEducationalType();
        }

        [HttpGet("get_person_add/{GUID}")]
        public object GetPersonAdd(string GUID)
        {
            return _repo.GetPersonAdd(GUID);
        }

        [HttpGet("child_intake_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<ChildInformationIntakeController>/5
        [HttpGet("{child_intake_guid}")]
        public object GetChildInformationIntake(string child_intake_guid)
        {
            return _repo.GetChildInformationIntake(child_intake_guid);
        }

        // POST api/<ChildInformationIntakeController>
        [HttpPost]
        public IActionResult Post([FromBody] ChildInformationIntakeModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<ChildInformationIntakeController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] ChildInformationIntakeModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<ChildInformationIntakeController>/5
        [HttpDelete("{solo_parent_intake}")]
        public IActionResult Delete(string solo_parent_intake)
        {
            var result = _repo.Delete(solo_parent_intake);

            if (!result) return BadRequest(new { response = "Delete general intake unsuccessful" });

            return Ok(new { response = "General intake deleted successfully" });
        }
    }
}
