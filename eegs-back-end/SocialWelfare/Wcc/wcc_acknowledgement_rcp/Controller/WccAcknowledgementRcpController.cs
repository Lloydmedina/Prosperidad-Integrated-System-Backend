using Microsoft.AspNetCore.Mvc;
using eegs_back_end.SocialWelfare.Wcc.wcc_acknowledgement_rcp.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_acknowledgement_rcp.Repository;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_acknowledgement_rcp.Controller
{
    [Route("social_welfare/wcc/acknowledgement")]
    [ApiController]
    public class WccAcknowledgementRcpController : ControllerBase
    {
        IWccAcknowledgementRcpRepository _repo;
        public WccAcknowledgementRcpController(IWccAcknowledgementRcpRepository repo)
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
        public IActionResult Post([FromBody] WccAcknowledgementRcpModel  model)
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
        public IActionResult UpdateDtl(string id, [FromBody] WccAcknowledgementRcpModel model)
        {
            if (!_repo.UpdateDetails(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }
            return Ok(new { result = "Successful..." });
        }
    }
}
