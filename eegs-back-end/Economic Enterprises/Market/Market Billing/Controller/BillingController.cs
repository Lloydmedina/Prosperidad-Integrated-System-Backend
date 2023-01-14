using eegs_back_end.Economic_Enterprises.Market.Market_Billing.Model;
using eegs_back_end.Economic_Enterprises.Market.Market_Billing.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eegs_back_end.Economic_Enterprises.Market.Market_Billing.Controller
{
    [Route("market/market-billing")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        IBillingRepository _repo;
        public BillingController(IBillingRepository repo)
        {
            _repo = repo;
        }
        [HttpGet]
        public object GetList([FromQuery] string dateFrom, [FromQuery] string dateTo)
        {
            return _repo.GetList(dateFrom, dateTo);
        }


        [HttpGet("utils")]
        public object getUtil([FromQuery] string id)
        {
            return _repo.GetUtilityRate(id);
        }


        [HttpGet("tenant-profiles")]
        public object GetProperties()
        {
            return _repo.GetTenantProfiles();
        }

        [HttpGet("{id}")]
        public BillingModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        [HttpPost]
        public IActionResult Post([FromBody] BillingModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] BillingModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }


        [HttpPut("{id}/post")]
        public IActionResult Post(string id)
        {
            if (_repo.Post(id))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id, [FromQuery] string remarks)
        {

            var result = _repo.Delete(id, remarks);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }


        [HttpGet("print/{id}")]
        public object GetPrintByID(string id)
        {

            return _repo.GetPrintByID(id);
        }
    }
}
