using eegs_back_end.Admin.PersonSetup.Model;
using eegs_back_end.Admin.PersonSetup.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Admin.PersonSetup.Controller
{
    [Route("admin/person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        byte[] FileAsByteArray;
        IPersonRepository _repo;
        public PersonController(IPersonRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<PersonController>
        [HttpGet]
        public List<object> Get()
        {
            return _repo.GetList();
        }

        [HttpGet("get_educational_type")]
        public List<object> GetEducationalType()
        {
            return _repo.GetEducationalType();
        }

        [HttpGet("get_region")]
        public List<object> GetRegion()
        {
            return _repo.GetRegion();
        }

        [HttpGet("get_active_list")]
        public List<object> GetListActive()
        {
            return _repo.GetListActive();
        }

        [HttpGet("get_active_list_above_sixty")]
        public List<object> GetListAboveSixty()
        {
            return _repo.GetListAboveSixty();
        }

        [HttpGet("get_deleted_list")]
        public List<object> GetListDeleted()
        {
            return _repo.GetListDeleted();
        }

        [HttpGet("get_list_generated")]
        public List<object> GetListGenerated([FromQuery] int filter_type_status_id, int status_id, int status_deleted_id, string this_month, string this_year, string monthly, string monthlyYear, string year_quarterly, int quarter, string yearly, string from, string to)
        {
            return _repo.GetListGenerated(filter_type_status_id, status_id, status_deleted_id, this_month, this_year, monthly, monthlyYear, year_quarterly, quarter, yearly, from, to);
        }

        [HttpGet("dropdown_values")]
        public List<object> GetDropDown()
        {
            return _repo.GetDropDown();
        }

        [HttpGet("dropdown_province")]
        public List<object> GetProvince()
        {
            return _repo.GetProvince();
        }

        [HttpGet("dropdown_prefix")]
        public List<object> GetPrefix()
        {
            return _repo.GetPrefix();
        }

        [HttpGet("blood_type")]
        public List<object> GetBloodType()
        {
            return _repo.GetBloodType();
        }

        [HttpGet("dropdown_city_mun/{province_id}")]
        public List<object> GetCityMun(int province_id)
        {
            return _repo.GetCityMun(province_id);
        }

        [HttpGet("dropdown_barangay/{city_mun_id}")]
        public List<object> GetBarangay(int city_mun_id)
        {
            return _repo.GetBarangay(city_mun_id);
        }

        // GET api/<PersonController>/5
        [HttpGet("{person_guid}")]
        public List<object> GetPerson(string person_guid)
        {
            return _repo.GetPerson(person_guid);
        }

        // POST api/<PersonController>
        [HttpPost]
        public IActionResult Post([FromBody] PersonModel model)
        {
            if(model.person_image != null)
            {
                var filePathName = "image" + "_" +
                DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                Path.GetExtension(model.person_file_name);
                var file_path = Path.Combine(Directory.GetCurrentDirectory(),
                    "Upload", filePathName);
                //model.person_image = "/StaticFiles/" + filePathName;

                if (model.person_image.Contains(","))
                {
                    model.person_image = model.person_image.Substring(model.person_image.IndexOf(",") + 1);
                    //FileAsByteArray = Convert.FromBase64String(value.person_image);
                }

                using (var fs = new FileStream(file_path, FileMode.CreateNew))
                {
                    byte[] info = System.Convert.FromBase64String(model.person_image);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }

            if (!_repo.Insert(model)) return BadRequest(new {result = "Failed..." });

            return Ok(new {result = "Successful..."});
        }

        // PUT api/<PersonController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] PersonModel value)
        {
            if (value.person_image != null)
            {
                var filePathName = "image" + "_" +
                DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
                Path.GetExtension(value.person_file_name);
                var file_path = Path.Combine(Directory.GetCurrentDirectory(),
                    "Upload", filePathName);
                value.person_base64 = file_path;

                if (value.person_image.Contains(","))
                {
                    value.person_image = value.person_image.Substring(value.person_image.IndexOf(",") + 1);
                    //FileAsByteArray = Convert.FromBase64String(value.person_image);
                }

                using (var fs = new FileStream(file_path, FileMode.CreateNew))
                {
                    byte[] info = System.Convert.FromBase64String(value.person_image);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }
            }

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });

            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<PersonController>/5
        [HttpDelete("{person_guid}")]
        public IActionResult Delete(string person_guid)
        {
            var result = _repo.Delete(person_guid);

            if (!result) return BadRequest(new { response = "Delete Person unsuccessful" });

            return Ok(new { response = "Person deleted successfully" });
        }

        [HttpPut("activate/{person_guid}")]
        public IActionResult Activate(string person_guid, [FromBody] PersonModel value)
        {
            var result = _repo.Activate(person_guid, value);

            if (!result) return BadRequest(new { response = "Activate Person unsuccessful" });

            return Ok(new { response = "Person activated successfully" });
        }
    }
}
