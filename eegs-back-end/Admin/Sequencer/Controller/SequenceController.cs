using eegs_back_end.Admin.Sequencer.Model;
using eegs_back_end.Admin.Sequencer.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.Sequencer.Controller
{
    [Route("admin/sequencer")]
    [ApiController]
    public class SequenceController : ControllerBase
    {
        ISequenceRepository _repo;

        public SequenceController(ISequenceRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<SequenceController>
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetFormList(id);
        }

        // POST api/<SequenceController>
        [HttpPost]
        public void Post([FromBody] SequenceModel value)
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
