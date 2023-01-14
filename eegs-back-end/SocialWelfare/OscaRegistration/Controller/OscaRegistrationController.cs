using eegs_back_end.Admin.OscaRegistrationSetup.Model;
using eegs_back_end.Admin.OscaRegistrationSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.OscaRegistrationSetup.Controller
{
    [Route("social_welfare/osca_registration")]
    [ApiController]
    public class OscaRegistrationController : ControllerBase
    {
        byte[] FileAsByteArray;
        IOscaRegistrationRepository _repo;
        public OscaRegistrationController(IOscaRegistrationRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<OscaRegistrationController>
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

        [HttpGet("get_employment_status")]
        public List<object> GetEmploymentStatus()
        {
            return _repo.GetEmploymentStatus();
        }

        [HttpGet("get_living_arrangement")]
        public List<object> GetLivingArrangement()
        {
            return _repo.GetLivingArrangement();
        }

        [HttpGet("get_classification")]
        public List<object> GetClassification()
        {
            return _repo.GetClassification();
        }

        [HttpGet("get_disability")]
        public List<object> GetListOfDisability()
        {
            return _repo.GetListOfDisability();
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

        [HttpGet("osca_registration_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<OscaRegistrationController>/5
        [HttpGet("{osca_registration_guid}")]
        public object GetOscaRegistration(string osca_registration_guid)
        {
            return _repo.GetOscaRegistration(osca_registration_guid);
        }

        // POST api/<OscaRegistrationController>
        [HttpPost]
        public IActionResult Post([FromBody] OscaRegistrationModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<OscaRegistrationController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] OscaRegistrationModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<OscaRegistrationController>/5
        [HttpDelete("{osca_registration_id}")]
        public IActionResult Delete(string osca_registration_id)
        {
            var result = _repo.Delete(osca_registration_id);

            if (!result) return BadRequest(new { response = "Delete OSCA registration unsuccessful" });

            return Ok(new { response = "OSCA deleted successfully" });
        }
    }
}
