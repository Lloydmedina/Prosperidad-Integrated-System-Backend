using eegs_back_end.Economic_Enterprises.Market.Delinquent_List.Model;
using eegs_back_end.Economic_Enterprises.Market.Delinquent_List.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Economic_Enterprises.Market.Delinquent_List.Controller
{
    [Route("market/delinquent")]
    [ApiController]
    public class DelinquentController : ControllerBase
    {
        IDelinquentRepository _repo;

        public DelinquentController(IDelinquentRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<DelinquentController>
        [HttpGet("generate")]
        public object Generate([FromQuery] DateTime date)
        {
            return _repo.Generate(date);
        }

        // GET api/<DelinquentController>/5
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }
        [HttpGet("{id}")]
        public object GetRecord(string id)
        {
            return _repo.GetRecord(id);
        }

        // POST api/<SequenceController>
        [HttpPost]
        public void Post([FromBody] DelinquentModel value)
        {
            _repo.Save(value);
        }

        // PUT api/<SequenceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<SequenceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
