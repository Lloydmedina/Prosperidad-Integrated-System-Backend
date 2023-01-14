using Microsoft.AspNetCore.Mvc;
using eegs_back_end.SocialWelfare.Wcc.wcc_case_conference.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_case_conference.Repository;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace eegs_back_end.SocialWelfare.Wcc.wcc_case_conference
{
    [Route("social_welfare/wcc/wcc-case-conference")]
    [ApiController]
    public class WccCaseConferenceController : ControllerBase
    {
        IWccCaseConferenceRepository _repo;

        public WccCaseConferenceController(IWccCaseConferenceRepository repo) { 
            _repo = repo;
        }
        [HttpGet]
        public List<object> GetList()
        {
            return _repo.GetList();
        }

        //GET CC DATA
        [HttpGet("get-data/{tid}")]
        public object GetData(string tid)
        {
            return _repo.GetData(tid);
        }
        //GET IR DATA
        [HttpGet("get-irdata/{tid}")]
        public object GetIRData(string tid)
        {
            return _repo.GetIRData(tid);
        }

        [HttpGet("get-case-dtl/{dtl_id}")]
        public List<object> GetCaseDtl(string dtl_id)
        {
            return _repo.GetCaseDtl(dtl_id);

        }

             //add new transaction
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] WccCaseConferenceModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }
        
        //update transaction details
        [HttpPut("update-detail/{id}")]
        public IActionResult UpdateDtl(string id, [FromBody] WccCaseConferenceModel model)
        {
            if (!_repo.UpdateDetails(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }
            return Ok(new { result = "Successful..." });
        }


    }
}
