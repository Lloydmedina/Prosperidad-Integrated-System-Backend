using eegs_back_end.Health.dental_certificate.Model;
using eegs_back_end.Health.dental_certificate.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace eegs_back_end.Health.dental_certificate
{
    [Route("health/dental-certificate")]
    [ApiController]
    public class DentalCertificateController : ControllerBase
    {
        IDentalCertificateRepository _repo;
        public DentalCertificateController(IDentalCertificateRepository repo) {
            _repo = repo;
        }

        [HttpGet]
        public List<object> GetList()
        {
            var data = _repo.GetList();

            return _repo.GetList();
        }

        [HttpGet("get-exam-dtl/{dtl_id}")]
        public List<object> GetExamData(string dtl_id)
        {
            return _repo.GetExamDatas(dtl_id);

        }
        [HttpGet("get-exam-dtlById/{dtl_id}")]
        public List<object> GetDataById(string dtl_id)
        {
            return _repo.GetDataById(dtl_id);

        }
        [HttpGet("{id}")]
        public List<Object> Get(string id)
        {
            return _repo.GetByID(id);
        }

        [HttpGet("dental-certificate-form-print/{id}")]
        public List<object> GetDatas(string ID)
        {
            return _repo.GetDatas(ID);
        }

        [HttpPost("add-new-dc")]
        public IActionResult Post([FromBody] NewDentalCertificate model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed ..." });

            return Ok(new { result = "Successful..." });
        }

        [HttpPut("update-dc/{id}")]
        public IActionResult UpdateTransaction(string id, [FromBody] List<DentalCertificateExam> model)
        {
            if (!_repo.UpdateTransaction(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }

            return Ok(new { result = "Successful..." });
        }

        [HttpPut("update-detail-byId/{id}")]
        public IActionResult UpdateDtlById(string id, [FromBody] DentalCertificateExam model)
        {
            if (!_repo.UpdateDtlById(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }


            return Ok(new { result = "Successful..." });
        }



        [HttpDelete("payedTransaction/{id}")]
        public IActionResult PayedTrans(string id)
        {
            var result = _repo.PayedTrans(id);

            if (!result) return BadRequest(new { response = "Saved unsuccessful" });

            return Ok(new { response = "Saved successfully" });
        }


        // DELETE api
        [HttpDelete("delete-details/{id}")]
        public IActionResult DeleteTrans(string id)
        {
            var result = _repo.DeleteTrans(id);

            if (!result) return BadRequest(new { response = "Transaction Delete unsuccessful" });

            return Ok(new { response = "Transaction  deleted successfully" });
        }
        //REVERT API
        [HttpDelete("revert-details/{id}")]
        public IActionResult RevertTrans(string id)
        {
            var result = _repo.RevertTrans(id);

            if (!result) return BadRequest(new { response = "Transaction revert unsuccessful" });

            return Ok(new { response = "Transaction  reverted successfully" });
        }
    }
}
