using eegs_back_end.Admin.SoloParentSetup.Model;
using eegs_back_end.Admin.SoloParentSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.SoloParentSetup.Controller
{
    [Route("social_welfare/solo_parent")]
    [ApiController]
    public class SoloParentController : ControllerBase
    {
        byte[] FileAsByteArray;
        ISoloParentRepository _repo;
        public SoloParentController(ISoloParentRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<SoloParentController>
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

        [HttpGet("get_deleted_list")]
        public List<object> GetListDeleted()
        {
            return _repo.GetListDeleted();
        }

        [HttpGet("get_list_generated")]
        public List<object> GetListGenerated([FromQuery] int filter_type_status_id, int status_id, int status_deleted_id, string this_month, string this_year, string monthly, string monthlyYear, string year_quarterly, int quarter, string yearly, string from, string to)
        {
            return _repo.GetListGenerated(filter_type_status_id, status_id, status_deleted_id, this_month, this_year, monthly, monthlyYear, year_quarterly, quarter, yearly, from, to);
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

        [HttpGet("solo_parent_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<SoloParentController>/5
        [HttpGet("{solo_parent_guid}")]
        public object GetSoloParent(string solo_parent_guid)
        {
            return _repo.GetSoloParent(solo_parent_guid);
        }

        // POST api/<SoloParentController>
        [HttpPost]
        public IActionResult Post([FromBody] SoloParentModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<SoloParentController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] SoloParentModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<SoloParentController>/5
        [HttpDelete("{general_intake_guid}")]
        public IActionResult Delete(string general_intake_guid)
        {
            var result = _repo.Delete(general_intake_guid);

            if (!result) return BadRequest(new { response = "Delete general intake unsuccessful" });

            return Ok(new { response = "General intake deleted successfully" });
        }
    }
}
