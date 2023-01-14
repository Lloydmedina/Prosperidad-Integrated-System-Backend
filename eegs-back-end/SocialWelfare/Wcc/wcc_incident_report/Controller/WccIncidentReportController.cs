using Microsoft.AspNetCore.Mvc;
using eegs_back_end.SocialWelfare.Wcc.wcc_incident_report.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_incident_report.Repository;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_incedent_report.Controller
{
    [Route("social_welfare/wcc/wcc-incident-report")]
    [ApiController]
    public class WccIncidentReportController : ControllerBase
    {
        IWccIncidentReportRepository _repo;
        public WccIncidentReportController(IWccIncidentReportRepository repo) {
            _repo = repo;
        }

        //GET INCIDENT REPORT LIST
        [HttpGet]
        public List<object> GetList()
        {
            return _repo.GetList();
        }
        //GET CASE LIST
        [HttpGet("get-case-list")]
        public List<object> GetCaseList()
        {
            return _repo.GetCaseList();
        }
        [HttpGet("get-case-byid/{caseId}")]
        public object GetCaseByid( string caseId)
        {
            return _repo.GetCaseByid(caseId);
        }
        //GET REPORT DATA
        [HttpGet("get-data/{tid}")]
        public object GetData(string tid)
        {
            return _repo.GetData(tid);
        }

        //GET REPORT DATA
        [HttpGet("get-dataRID/{rid}")]
        public object GetDataRID(string rid)
        {
            return _repo.GetDataRID(rid);
        }

              //add new transaction
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] WccIncidentReportModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        //update transaction details
        [HttpPut("update-detail/{id}")]
        public IActionResult UpdateDtl(string id, [FromBody] WccIncidentReportModel model)
        {
            if (!_repo.UpdateDetails(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }
            return Ok(new { result = "Successful..." });
        }
    }
}
