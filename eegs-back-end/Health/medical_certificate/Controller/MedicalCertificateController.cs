using eegs_back_end.Health.medical_certificate.Model;
using eegs_back_end.Health.medical_certificate.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.medical_certificate
{
    [Route("health/medical-certificate")]
    [ApiController]
    public class MedicalCertificateController : ControllerBase
    {
        IMedicalCertificateRepository _repo;
        public MedicalCertificateController(IMedicalCertificateRepository repo) {
            _repo = repo;
        }

        [HttpGet]
        public List<object> GetList() {
            var data = _repo.GetList();

            return _repo.GetList();
        }
        
        [HttpGet("get-exam-dtl/{dtl_id}")]
        public List<object> GetExamData(string dtl_id)
        {
            return _repo.GetExamData(dtl_id);

        }

        [HttpGet("{id}")]
        public List<Object> Get(string id)
        {
            return _repo.GetByID(id);
        }
        [HttpGet("medical-certificate-form-print/{id}")]
        public List<object> GetDatas(string ID)
        {
            return _repo.GetDatas(ID);
        }

        [HttpPost("add-new-mc")]
        public IActionResult Post([FromBody] NewMedicalCertificate model) {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed ..." });

            return Ok(new { result = "Successful..."});
        }

        [HttpPut("update-detail-byId/{id}")]
        public IActionResult UpdateDtlById(string id, [FromBody] MedicalCertificateExam model)
        {
            if (!_repo.UpdateDtlById(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }


            return Ok(new { result = "Successful..." });
        }

        [HttpGet("get-details-byId/{trans_id}")]
        public List<object> GetExamListData(string trans_id)
        {
            return _repo.GetExamListData(trans_id);

        }

        [HttpPut("update-detail/{id}")]
        public IActionResult UpdateDtl(string id, [FromBody] List<MedicalCertificateExam> model)
        {
            if (!_repo.UpdateDetails(id, model))
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
