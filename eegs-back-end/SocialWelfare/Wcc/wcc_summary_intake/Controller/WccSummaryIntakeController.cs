using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eegs_back_end.SocialWelfare.Wcc.wcc_summary_intake.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_summary_intake.Repository;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_summary_intake.Controller
{
    [Route("social-welfare/wcc/wcc-summary-intake")]
    [ApiController]
    public class WccSummaryIntakeController : ControllerBase
    {
        IWccSummaryIntakeRepository _repo;
        public WccSummaryIntakeController(IWccSummaryIntakeRepository repo) { 
            _repo = repo;
        }

        //GET SUMMARY INTAKE REPORT LIST
        [HttpGet]
        public List<object> GetList()
        {
            return _repo.GetList();
        }

        //GET SUMMARY DATA
        [HttpGet("get-data/{tid}")]
        public object GetData(string tid)
        {
            return _repo.GetData(tid);
        }

        //GET CC DATA
        [HttpGet("get-cc-data/{regid}")]
        public object GetCCData(string regid)
        {
            return _repo.GetCCData(regid);
        }
        [HttpGet("get-case-dtl/{dtl_id}")]
        public List<object> GetCaseDtl(string dtl_id)
        {
            return _repo.GetCaseDtl(dtl_id);

        }

                 //add new transaction
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] WccSummaryIntakeModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }
    }
}
