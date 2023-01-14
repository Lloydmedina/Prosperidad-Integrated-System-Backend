using Microsoft.AspNetCore.Mvc;
using eegs_back_end.SocialWelfare.Wcc.wcc_intervention_undertaken.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_intervention_undertaken.Repository;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eegs_back_end.SocialWelfare.Wcc.wcc_intervention_undertaken.Controller
{
    [Route("social_welfare/wcc/wcc-intervention-undertaken")]
    [ApiController]

    public class WccInterventionUndertakenController : ControllerBase
    {
        IWccInterventionUndertakenRepository _repo;
    public WccInterventionUndertakenController(IWccInterventionUndertakenRepository repo)
    {
        _repo = repo;
    }
         [HttpGet]
        public List<object> GetList()
        {
            return _repo.GetList();
        }
         //get IU list
        [HttpGet("get-IUlist")]
        public List<object> GetIUlist()
        {
            return _repo.GetIUlist();
        }

        //add new transaction
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] NewWccInterventionUndertaken model)
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
    }
}
