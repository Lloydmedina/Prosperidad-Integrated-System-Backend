using eegs_back_end.Health.health_card.Model;
using eegs_back_end.Health.health_card.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.health_card.Controller
{
    [Route("health/health-card")]
    [ApiController]
    public class HealthCardController : ControllerBase
    {
        IHealthCardRepository _repo;
        public HealthCardController(IHealthCardRepository repo)
        {
            _repo = repo;
        }

        // GET: api/<HealthCardController>


        [HttpGet]
        public object GetList()
        {
            var data = _repo.GetList();
            if (data == null)
                return null;
            return data;
        }

        // GET api/<HealthCardController>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetByID(id);
        }

        // POST api/<HealthCardController>


        // PUT api/<DomainController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] HealthCardModel value)
        {

            var result = _repo.Edit(id, value);

            if (!result) return BadRequest(new { response = "Edit Domain unsuccessful" });

            return Ok(new { response = "Domain update successful" });
        }

        // DELETE api/<DomainController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Delete Domain unsuccessful" });

            return Ok(new { response = "Domain deleted successfully" });
        }

        [HttpGet("project")]
        public List<object> GetProject()
        {
            var data = _repo.GetProject();
            if (data == null)
                return null;
            return data;
        }
        

        [HttpGet("transactions/{GUID}")]
        public List<object> GetTransactionsInfo
            (string GUID)
        {
            return _repo.GetTransactionsInfo(GUID);

        }
        [HttpGet("get-trans-data/{GUID}")]
        public List<object> GetTransactionsData
           (string GUID)
        {
            return _repo.GetTransactionsData(GUID);

        }
        [HttpGet("checkTransactions/{GUID}")]
        public object CheckTransactionsInfo
           (string GUID)
        {
            return _repo.CheckTransactionsInfo(GUID);

        }
        //HC history
        [HttpGet("transactionsHistory/{GUID}")]
        public List<object> TransactionsHistory(string GUID)
        {
             var data = _repo.TransactionsHistory(GUID);
            if (data == null)
                return null;
            return data;


        }

        /*GET PERSON DETAILS*/
        [HttpGet("get-person-data/{GUID}")]
        public object GetPersonData(string GUID)
        {
            return _repo.GetPersonData(GUID);

        }
        /*GET TRANSACTION DETAILS*/
        [HttpGet("get-exam-dtl/{dtl_id}")]
        public List<object> GetExamData(string dtl_id)
        {
            return _repo.GetExamData(dtl_id);

        }
        [HttpGet("medical_exam_list/{code}")]
        public List<object> GetMedicalExamData(string code)
        {
            var data = _repo.GetMedicalExamData(code);
            if (data == null)
                return null;
            return data;

        }
        /*GET MEDICAL EXAM TRANSACTIONS*/
        [HttpGet("medical_trans_exam_list")]
        public List<object> GetMedical_Trans_ExamData([FromQuery] string g, [FromQuery] string med)
        {
            var data = _repo.GetMedical_Trans_ExamData(g, med);
            if (data == null)
                return null;
            return data;

        }


        /*ADD NEW MEDICAL TRANSACTION EXAM*/
        [HttpPost("add-new-med-exam")]
        public IActionResult Post([FromBody] NewMedicalTransactionModel model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }


        /*ADD NEW HEALTH CARD TRANSACTION */
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] NewTransactionModel model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }
        [HttpPost("add-payment")]
        public IActionResult Post([FromBody] HealthCardPayment model) {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });
            return Ok(new { result = "Successful..." });
        }

        [HttpGet("get-guid")]
        public object generateGuid() {
            return new { guid = Guid.NewGuid().ToString() };
           
        }

        [HttpGet("get-heath-card-id")]
        public object generateHealthCardId() {
            return _repo.Generate_Health_Card_id();
        }


        [HttpPut("update-detail/{id}")]
        public IActionResult UpdateDtl(string id, [FromBody]List<NewMedicalTransactionModel> model)
        {
            if (!_repo.UpdateDetails(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }


            return Ok(new { result = "Successful..." });
        }

        [HttpPut("update-detail-byId/{id}")]
        public IActionResult UpdateDtlById(string id, [FromBody] NewMedicalTransactionModel model)
        {
            if (!_repo.UpdateDtlById(id, model))
            {
                return BadRequest(new { result = "Failed..." });
            }


            return Ok(new { result = "Successful..." });
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

        [HttpDelete("payedTransaction/{id}")]
        public IActionResult PayedTrans(string id)
        {
            var result = _repo.PayedTrans(id);

            if (!result) return BadRequest(new { response = "Transaction saved unsuccessful" });

            return Ok(new { response = "Transaction  saved successfully" });
        }

    }
}
