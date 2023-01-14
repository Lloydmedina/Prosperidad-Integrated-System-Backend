using eegs_back_end.Admin.FinancialAssistanceSetup.Model;
using eegs_back_end.Admin.FinancialAssistanceSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.FinancialAssistanceSetup.Controller
{
    [Route("social_welfare/financial_assistance")]
    [ApiController]
    public class FinancialAssistanceController : ControllerBase
    {
        byte[] FileAsByteArray;
        IFinancialAssistanceRepository _repo;
        public FinancialAssistanceController(IFinancialAssistanceRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<FinancialAssistanceController>
        [HttpGet]
        public List<object> Get(int filter_type_status_id, int rec_id, string this_month, string this_year, string monthly, string monthlyYear, string year_quarterly, int quarter, string yearly, string from, string to)
        {
            return _repo.GetList(filter_type_status_id, rec_id, this_month, this_year, monthly, monthlyYear, year_quarterly, quarter, yearly, from, to);
        }

        [HttpGet("get_all_list")]
        public List<object> GetAllList()
        {
            return _repo.GetAllList();
        }

        [HttpGet("get_form_settings")]
        public List<object> getFormSettings()
        {
            return _repo.getFormSettings();
        }

        [HttpGet("get_disability")]
        public List<object> GetListOfDisability()
        {
            return _repo.GetListOfDisability();
        }

        [HttpGet("get_application")]
        public List<object> GetListOfApplication()
        {
            return _repo.GetListOfApplication();
        }

        [HttpGet("get_application_type")]
        public List<object> GetListOfApplicationType()
        {
            return _repo.GetListOfApplicationType();
        }

        [HttpGet("get_educational_type")]
        public List<object> GetEducationalType()
        {
            return _repo.GetEducationalType();
        }

        [HttpGet("get_recommendations")]
        public List<object> GetListOfRecommendations()
        {
            return _repo.GetListOfRecommendations();
        }

        [HttpGet("get_person_add/{GUID}")]
        public object GetPersonAdd(string GUID)
        {
            return _repo.GetPersonAdd(GUID);
        }

        [HttpGet("FinancialAssistance_details/{GUID}")]
        public List<object> GetDetails(string GUID)
        {
            return _repo.GetDetails(GUID);
        }

        [HttpGet("history_logs/{GUID}")]
        public object GetHistoryLogs(string GUID)
        {
            return _repo.GetHistoryLogs(GUID);
        }

        // GET api/<FinancialAssistanceController>/5
        [HttpGet("{aics_voucher_guid}")]
        public object GetFinancialAssistance(string aics_voucher_guid)
        {
            return _repo.GetFinancialAssistance(aics_voucher_guid);
        }

        // POST api/<FinancialAssistanceController>
        [HttpPost]
        public IActionResult Post([FromBody] FinancialAssistanceModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // PUT api/<FinancialAssistanceController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] FinancialAssistanceModel value)
        {

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<FinancialAssistanceController>/5
        [HttpDelete("{aics_voucher_guid}")]
        public IActionResult Delete(string aics_voucher_guid)
        {
            var result = _repo.Delete(aics_voucher_guid);

            if (!result) return BadRequest(new { response = "Delete general intake unsuccessful" });

            return Ok(new { response = "General intake deleted successfully" });
        }
    }
}
