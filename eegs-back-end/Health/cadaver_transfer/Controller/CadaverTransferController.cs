using eegs_back_end.Health.cadaver_transfer.Model;
using eegs_back_end.Health.cadaver_transfer.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.cadaver_transfer.Controller
{
    [Route("health/cadaver-transfer")]
    [ApiController]
    public class CadaverTransferController : ControllerBase
    {
        ICadaverTransferRepository _repo;
        public CadaverTransferController(ICadaverTransferRepository repo) { 
            _repo = repo;
        }
        [HttpGet]
        public List<object> GetList()
        {
            var data = _repo.Getlist();
            if (data == null)
                return null;
            return data;
        }
        [HttpGet("get-person-data/{ID}")]
        public List<object> GetPersonData(string ID)
        {
            return _repo.GetPersonData(ID);
        }
        [HttpPost("add-new-ctp")]
        public IActionResult Post([FromBody] NewCadaverTransferModel model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed ..." });

            return Ok(new { result = "Successful..." });
        }
        [HttpGet("get-list-byid/{ID}")]
        public List<object> GetListById(string ID)
        {
            return _repo.GetListById(ID);
        }

        [HttpPut("update-ctp/{id}")]
        public IActionResult UpdateTransaction(string id, [FromBody] NewCadaverTransferModel model)
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
