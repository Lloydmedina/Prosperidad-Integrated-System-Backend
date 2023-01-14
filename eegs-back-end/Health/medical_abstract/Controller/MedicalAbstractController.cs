using eegs_back_end.Health.medical_abstract.Model;
using eegs_back_end.Health.medical_abstract.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.medical_abstract
{
    [Route("health/medical-abstract")]
    [ApiController]
    public class MedicalAbstractController : ControllerBase
    {
        IMedicalAbstractRepository _repo;
        public MedicalAbstractController(IMedicalAbstractRepository repo) {
            _repo = repo;
        }

        [HttpPost("add-new-ma")]
        public IActionResult Post([FromBody] NewMedicalAbstract model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed ..." });

            return Ok(new { result = "Successful..." });
        }
        [HttpGet]
        public List<object> GetList()
        {
            var data = _repo.GetList();
            if (data == null)
                return null;
            return data;
        }
        [HttpGet("{id}")]
        public List<object> GetById(string ID)
        {
        return _repo.GetById(ID);
        }
        [HttpGet("get-transactions-byId/{id}")]
        public List<object> GetTransById(string ID)
        {
            return _repo.GetById(ID);
        }

        [HttpGet("medicine-type")]
        public List<object> GetMedicineTypeList()
        {
            return _repo.GetMedicineTypeList();
            
        }
         [HttpGet("signatory")]
        public List<object> Getsignatory()
        {
            return _repo.Getsignatory();
            
        }

        [HttpPut("update-ma/{id}")]
        public IActionResult UpdateDtl(string id, [FromBody] MedicalAbstractDtl model)
        {
            if (!_repo.UpdateDtl(id, model))
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
