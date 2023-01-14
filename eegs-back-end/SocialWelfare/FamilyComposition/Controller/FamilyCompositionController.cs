using eegs_back_end.Admin.FamilyCompositionSetup.Model;
using eegs_back_end.Admin.FamilyCompositionSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.FamilyCompositionSetup.Controller
{
    [Route("social_welfare/family_composition")]
    [ApiController]
    public class FamilyCompositionController : ControllerBase
    {
        byte[] FileAsByteArray;
        IFamilyCompositionRepository _repo;
        public FamilyCompositionController(IFamilyCompositionRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<FamilyCompositionController>
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

        [HttpGet("get_registered_fourps")]
        public List<object> GetRegisteredFourps()
        {
            return _repo.GetRegisteredFourps();
        }

        [HttpGet("get_registered_ips")]
        public List<object> GetRegisteredIPS()
        {
            return _repo.GetRegisteredIPS();
        }

        [HttpGet("get_list_generated")]
        public List<object> GetListGenerated([FromQuery] int filter_type_status_id, int status_id, int status_deleted_id, string this_month, string this_year, string monthly, string monthlyYear, string year_quarterly, int quarter, string yearly, string from, string to)
        {
            return _repo.GetListGenerated(filter_type_status_id, status_id, status_deleted_id, this_month, this_year, monthly, monthlyYear, year_quarterly, quarter, yearly, from, to);
        }

        [HttpGet("servertime")]
        public object ServerTime()
        {
            return _repo.ServerTime();
        }

        [HttpGet("get_educational_type")]
        public List<object> GetEducationalType()
        {
            return _repo.GetEducationalType();
        }

        [HttpGet("family_head_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("family_head_details_with_person_guid/{GUID}")]
        public List<object> GetDetailsUsingPersonGUID(string GUID)
        {
            return _repo.GetDetailsUsingPersonGUID(GUID);
        }

        // GET api/<FamilyCompositionController>/5
        [HttpGet("{FamilyComposition_guid}")]
        public object GetFamilyComposition(string FamilyComposition_guid)
        {
            return _repo.GetFamilyComposition(FamilyComposition_guid);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // POST api/<FamilyCompositionController>
        [HttpPost]
        public IActionResult Post([FromBody] FamilyCompositionModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<FamilyCompositionController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] FamilyCompositionModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<FamilyCompositionController>/5
        [HttpDelete("{family_composition_guid}")]
        public IActionResult Delete(string family_composition_guid)
        {
            var result = _repo.Delete(family_composition_guid);

            if (!result) return BadRequest(new { response = "Delete family composition unsuccessful" });

            return Ok(new { response = "Family composition deleted successfully" });
        }
    }
}
