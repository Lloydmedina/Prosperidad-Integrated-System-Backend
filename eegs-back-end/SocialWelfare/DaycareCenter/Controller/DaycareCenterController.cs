using eegs_back_end.Admin.DaycareCenterSetup.Model;
using eegs_back_end.Admin.DaycareCenterSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.DaycareCenterSetup.Controller
{
    [Route("social_welfare/daycare_center")]
    [ApiController]
    public class DaycareCenterController : ControllerBase
    {
        byte[] FileAsByteArray;
        IDaycareCenterRepository _repo;
        public DaycareCenterController(IDaycareCenterRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<DaycareCenterController>
        [HttpGet]
        public List<object> Get()
        {
            return _repo.GetList();
        }

        [HttpGet("get_all_list")]
        public List<object> GetAllList()
        {
            return _repo.GetAllList();
        }

        [HttpGet("get_educational_type")]
        public List<object> GetEducationalType()
        {
            return _repo.GetEducationalType();
        }

        [HttpGet("get_occupation_list")]
        public List<object> GetOccupationList()
        {
            return _repo.GetOccupationList();
        }

        [HttpGet("get_disability")]
        public List<object> GetListOfDisability()
        {
            return _repo.GetListOfDisability();
        }

        [HttpGet("get_employment_status")]
        public List<object> GetEmploymentStatus()
        {
            return _repo.GetEmploymentStatus();
        }

        [HttpGet("get_employment_type")]
        public List<object> GetEmploymentType()
        {
            return _repo.GetEmploymentType();
        }

        [HttpGet("get_employer_type")]
        public List<object> GetEmployerType()
        {
            return _repo.GetEmployerType();
        }

        [HttpGet("get_cause_of_disability")]
        public List<object> GetCauseOfDisability()
        {
            return _repo.GetCauseOfDisability();
        }

        [HttpGet("get_ph_membership")]
        public List<object> GetListOfApplication()
        {
            return _repo.GetPhilhealthMembership();
        }

        [HttpGet("get_person_add/{GUID}")]
        public object GetPersonAdd(string GUID)
        {
            return _repo.GetPersonAdd(GUID);
        }

        [HttpGet("child_info_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<DaycareCenterController>/5
        [HttpGet("{daycare_center_guid}")]
        public object GetDaycareCenter(string daycare_center_guid)
        {
            return _repo.GetDaycareCenter(daycare_center_guid);
        }

        // POST api/<DaycareCenterController>
        [HttpPost]
        public IActionResult Post([FromBody] DaycareCenterModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<DaycareCenterController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] DaycareCenterModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<DaycareCenterController>/5
        [HttpDelete("{general_intake_guid}")]
        public IActionResult Delete(string general_intake_guid)
        {
            var result = _repo.Delete(general_intake_guid);

            if (!result) return BadRequest(new { response = "Delete general intake unsuccessful" });

            return Ok(new { response = "General intake deleted successfully" });
        }
    }
}
