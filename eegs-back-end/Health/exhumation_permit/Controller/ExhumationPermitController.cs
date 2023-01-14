using Microsoft.AspNetCore.Mvc;
using eegs_back_end.Health.exhumation_permit.Model;
using eegs_back_end.Health.exhumation_permit.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.exhumation_permit
{
[Route("health/exhumation-permit")]
[ApiController]
public class ExhumationPermitController : ControllerBase
{
        IExhumationPermitRepository _repo;

        public ExhumationPermitController(IExhumationPermitRepository repo) {
            _repo = repo;
        }
        [HttpGet]
        public List<object> GetList() {
            var data = _repo.GetList();
            return _repo.GetList();
        }
        [HttpGet("get-list-byid/{ID}")]
        public List<object> GetListById(string ID)
        {
            return _repo.GetListById(ID);
        }

        [HttpGet("get-person-data/{ID}")]
        public List<object> GetPersonData(string ID)
        {
            return _repo.GetPersonData(ID);
        }

        [HttpPost("add-new-exp")]
        public IActionResult Post([FromBody] NewExPermitModel model)
        {
            if (!_repo.Insert(model)) return BadRequest( new { result = "Failed ..."}) ;
            return Ok(new { result = "Successful ..."});
        }

        [HttpPut("update-exp/{id}")]
        public IActionResult UpdateTransaction(string id, [FromBody] NewExPermitModel model)
        {
            if (!_repo.UpdateTransaction(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }

            return Ok(new { result = "Successful..." });
        }



        //CHANGE TRANSACTION STATUS
        [HttpDelete("payedTransaction/{id}")]
        public IActionResult PayedTrans(string id)
        {
            var result = _repo.PayedTrans(id);

            if (!result) return BadRequest(new { response = "Saved unsuccessful" });

            return Ok(new { response = "Saved successfully" });
        }


        // DELETE TRANSACTION API
        [HttpDelete("delete-details/{id}")]
        public IActionResult DeleteTrans(string id)
        {
            var result = _repo.DeleteTrans(id);

            if (!result) return BadRequest(new { response = "Transaction Delete unsuccessful" });

            return Ok(new { response = "Transaction  deleted successfully" });
        }
        //REVERT TRANSACTION API
        [HttpDelete("revert-details/{id}")]
        public IActionResult RevertTrans(string id)
        {
            var result = _repo.RevertTrans(id);

            if (!result) return BadRequest(new { response = "Transaction revert unsuccessful" });

            return Ok(new { response = "Transaction  reverted successfully" });
        }
    }
}

