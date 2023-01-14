using eegs_back_end.DbModule;
using eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Model;
using eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Market.Tenant_Profiling.Controller
{
    [Route("market/tenant-profiling")]
    [ApiController]
    public class TenantProfileController : ControllerBase
    {
        ITenantProfileRepository _repo;
        public TenantProfileController(ITenantProfileRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<TenantProfileController>
        [HttpGet]
        public object GetList([FromQuery] int status_id)
        {
            return _repo.GetList(status_id);
        }

        [HttpGet("dropdown")]
        public object getDropdown()
        {
            return _repo.GetDropDown();
        }


        [HttpGet("{id}")]
        public TenantProfileModel Get(string id)
        {

            return _repo.GetByID(id);
        }

        // POST api/<BarangayOfficialController>
        [HttpPost]
        public IActionResult Post([FromBody] TenantProfileModel value)
        {
            if (_repo.Insert(value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // PUT api/<BarangayOfficialController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] TenantProfileModel value)
        {
            if (_repo.Edit(id, value))
                return Ok(new { result = "Successful" });

            return BadRequest(new { result = "Unsuccessful" });
        }

        // DELETE api/<BarangayOfficialController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id, [FromQuery] string remarks)
        {

            var result = _repo.Delete(id, remarks);

            if (!result) return BadRequest(new { response = "Unsuccessful" });

            return Ok(new { response = "Successful" });
        }

        [HttpGet("lgu-details")]
        public object LGUDetails([FromQuery]string brgy_id)
        {
            string sql = "select * from general_address.lgu_brgy_setup_temp where brgy_id = " + brgy_id;
            return QueryModule.DataObject<object>(sql);
        }

        [HttpGet("print-list")]
        public object GetPrintList()
        {
            return _repo.GetPrintList();
        }
    }
}
