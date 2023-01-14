using eegs_back_end.Health.sanitary_permit.Model;
using eegs_back_end.Health.sanitary_permit.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.sanitary_permit
{
    [Route("health/sanitary-permit")]
    [ApiController]
    public class SanitaryPermitController : ControllerBase
    {
        ISanitaryPermitRepository _repo;
        public SanitaryPermitController(ISanitaryPermitRepository repo) {
            _repo = repo;
        }
        [HttpGet]
        public object GetList()
        {
            var data = _repo.GetList();
            if (data == null)
                return null;
            return data;
        }
        [HttpGet("trans_data/{ID}")]
        public object GetTransData(string ID)
        {
            return _repo.GetTransData(ID);
        }
        [HttpGet("business_list")]
        public List<object> GetBusinessList() {
            var data = _repo.GetBusinessList();

            return _repo.GetBusinessList();
        }

        [HttpGet("business_data/{ID}")]
        public object GetBusinessData(string ID)
        {
            return _repo.GetBusinessData(ID);
        }
        [HttpGet("transaction_list")]
        public List<object> GetTransactionList()
        {
            var data = _repo.GetTransactionList();

            return _repo.GetTransactionList();
        }
        [HttpGet("transaction_list_byid/{id}")]
        public List<object> CheckTrans( string ID)
        {
            return _repo.CheckTrans(ID);
        }
        
         [HttpGet("inspector_list")]
        public List<object> GetInspectorList()
        {
            var data = _repo.GetInspectorList();

            return _repo.GetInspectorList();
        }
        [HttpGet("inspector_data/{ID}")]
        public object GetInspectorData(string ID)
        {
            return _repo.GetInspectorData(ID);
        }

        [HttpPost("add-new-sp")]
        public IActionResult Post([FromBody] SPTransactionModel model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed ..." });

            return Ok(new { result = "Successful..." });
        }
        [HttpPut("update-sp/{id}")]
        public IActionResult UpdateTransaction(string id, [FromBody] SPTransactionModel model)
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
