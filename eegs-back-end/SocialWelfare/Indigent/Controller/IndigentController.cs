using eegs_back_end.Admin.IndigentSetup.Model;
using eegs_back_end.Admin.IndigentSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.IndigentSetup.Controller
{
    [Route("social_welfare/indigent")]
    [ApiController]
    public class IndigentController : ControllerBase
    {
        byte[] FileAsByteArray;
        IIndigentRepository _repo;
        public IndigentController(IIndigentRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<IndigentController>
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

        [HttpGet("indigent_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<IndigentController>/5
        [HttpGet("{indigent_guid}")]
        public object GetIndigent(string Indigent_guid)
        {
            return _repo.GetIndigent(Indigent_guid);
        }

        // POST api/<IndigentController>
        [HttpPost]
        public IActionResult Post([FromBody] IndigentModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<IndigentController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] IndigentModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<IndigentController>/5
        [HttpDelete("{indigent_guid}")]
        public IActionResult Delete(string indigent_guid)
        {
            var result = _repo.Delete(indigent_guid);

            if (!result) return BadRequest(new { response = "Delete general intake unsuccessful" });

            return Ok(new { response = "General intake deleted successfully" });
        }
    }
}
