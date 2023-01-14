using Microsoft.AspNetCore.Mvc;
using eegs_back_end.SocialWelfare.Wcc.wcc_registration.Model;
using eegs_back_end.SocialWelfare.Wcc.wcc_registration.Repository;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace eegs_back_end.SocialWelfare.Wcc.wcc_registration.Controller
{
    [Route("social_welfare/wcc/registration")]
    [ApiController]
    public class WccRegistrationController : ControllerBase
    {
        IWccRegistrationRepository _repo;
        public WccRegistrationController(IWccRegistrationRepository repo) {
            _repo = repo;
        }

        [HttpGet]
        public List<object> GetList()
        {
            return _repo.GetList();
        }
        //get family details
        [HttpGet("get-fcid/{pid}")]
        public object GetFC(string pid)
        {
            return _repo.GetFC(pid);
        }
        //get transaction details
        [HttpGet("get-data/{tid}")]
        public object GetData(string tid)
        {
            return _repo.GetData(tid);
        }
        //get family members
        [HttpGet("get-family-list/{fcid}")]
        public List<object> GetFCList(string fcid)
        {
            return _repo.GetFCList(fcid);
        }

        //get family head
        [HttpGet("get-family-head/{pid}")]
        public object GetFCHead(string pid)
        {
            return _repo.GetFCHead(pid);
        }
        //get referrer
        [HttpGet("get-referrer/{pid}")]
        public object GetReferrer(string pid)
        {
            return _repo.GetReferrer(pid);
        }

        //add new transaction
        [HttpPost("add-new-wccadmission")]
        public IActionResult Post([FromBody] WccRegistrationModel model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }
        //update transaction
        [HttpPut("update-wcccadmission/{id}")]
        public IActionResult UpdateData(string id, [FromBody] WccRegistrationModel model)
        {
            if (!_repo.UpdateData(id, model)) {
                return BadRequest(new { result = "Failed...." });
            }
            return Ok(new { result = "Successful...." });
        }


    }
}
