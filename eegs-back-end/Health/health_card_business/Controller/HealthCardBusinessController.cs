using eegs_back_end.Health.health_card_business.Model;
using eegs_back_end.Health.health_card_business.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.health_card_business.Controller
{
    [Route("health/healthcard-business")]
    [ApiController]
    public class HealthCardBusinessController : ControllerBase
    {
        IHealthCardBusinessRepository _repo;
        public HealthCardBusinessController(IHealthCardBusinessRepository repo) {
            _repo = repo;
        }
        [HttpGet]
        public List<object> GetList()
        {
            var data = _repo.GetList();
            if (data == null)
                return null;
            return data;
        }

        [HttpGet("business_data/{ID}")]
        public List<object> GetBusinessData(string ID)
        {
            return _repo.GetBusinessData(ID);
        }

        /*ADD NEW HEALTH CARD TRANSACTION */
        [HttpPost("add-new")]
        public IActionResult Post([FromBody] NewTransactionModel model)
        {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        [HttpGet("checkBusiness/{GUID}")]
        public object CheckBusinessInfo
       (string GUID)
        {
            return _repo.CheckBusinessInfo(GUID);

        }
    }
}
