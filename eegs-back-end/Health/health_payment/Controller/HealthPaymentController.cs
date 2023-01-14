using eegs_back_end.Health.health_payment.Model;
using eegs_back_end.Health.health_payment.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Health.health_payment.Controller
{
    [Route("health/health-payment")]
    [ApiController]
    public class HealthPaymentController : ControllerBase
    {
        IHealthPaymentRepository _repo;
        public HealthPaymentController(IHealthPaymentRepository repo)
        {
            _repo = repo;
        }

        // GET: api/<HealthCardController>


        [HttpPost("add-payment")]
        public IActionResult Post([FromBody] HealthCardPayment model) {
            if (!_repo.Insert(model)) return BadRequest(new { result = "Failed..." });
            return Ok(new { result = "Successful..." });
        }

        
    }
}
