using Microsoft.AspNetCore.Mvc;
using eegs_back_end.SocialWelfare.Wcc.wcc_discharge.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_discharge.Repository;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eegs_back_end.SocialWelfare.Wcc.wcc_turnover_of_custody.Model;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_discharge.Controller
{
    [Route("social_welfare/wcc/wcc-discharge")]
    [ApiController]
    public class WccDischargeController : ControllerBase
    {
        IWccDischargeRepository _repo;
        public WccDischargeController(IWccDischargeRepository repo)
        { 
            _repo = repo;
        }

        [HttpGet]
        public List<object> GetList()
        {
            return _repo.GetList();
        }
        //add new transaction
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] WccDischargeModel model)
        {

            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }
        //get transaction details
        [HttpGet("get-data/{tid}")]
        public List<object> GetData(string tid)
        {
            return _repo.GetData(tid);
        }

        //update transaction details
        [HttpPut("update-detail/{id}")]
        public IActionResult UpdateDtl(string id, [FromBody] WccDischargeModel model)
        {
            if (!_repo.UpdateDetails(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }
            return Ok(new { result = "Successful..." });
        }
    }
}
