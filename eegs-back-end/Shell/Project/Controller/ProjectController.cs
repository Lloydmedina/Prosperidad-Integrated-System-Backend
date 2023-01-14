using eegs_back_end.Shell.Project.Model;
using eegs_back_end.Shell.Project.Repository;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.Project.Controller
{
    [Route("project-lgu")]
    [ApiController]
    public class ProjectController : ControllerBase
    {
        IProjectRepository _repo;
        public ProjectController(IProjectRepository repo)
        {
            _repo = repo;
        }
        // GET: api/<ProjectController>
        [HttpGet]
        public object Get()
        {
            return _repo.GetList();
        }

        // GET api/<ProjectController>/5
        [HttpGet("{id}")]
        public object Get(string id)
        {
            return _repo.GetByID(id);
        }
        [HttpGet("config")]
        public object GetConfig()
        {
            return _repo.GetConfigValues();
        }

        // POST api/<ProjectController>
        [HttpPost]
        public IActionResult Post([FromBody] ProjectModel value)
        {
            if (value.report_config.header_logo1 != null && value.report_config.header_logo1 != "")
            {
                value.report_config.base_64_1 = processPath(value.report_config.file_name1);
                value.report_config.header_logo1 = processImg(value.report_config.header_logo1, value.report_config.base_64_1);
            }

            if (value.report_config.header_logo2 != null && value.report_config.header_logo2 != "")
            {
                value.report_config.base_64_2 = processPath(value.report_config.file_name2);
                value.report_config.header_logo2 = processImg(value.report_config.header_logo2, value.report_config.base_64_2);
            }

            if (!_repo.Insert(value)) return BadRequest(new { result = "Failed..." });


            return Ok(new { result = "Successful..." });
        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public IActionResult Put(string id, [FromBody] ProjectModel value)
        {
            if (value.report_config.header_logo1 != null && value.report_config.header_logo1 != "")
            {
                value.report_config.base_64_1 = processPath(value.report_config.file_name1);
                value.report_config.header_logo1 = processImg(value.report_config.header_logo1, value.report_config.base_64_1);
            }

            if (value.report_config.header_logo2 != null && value.report_config.header_logo2 != "")
            {
                value.report_config.base_64_2 = processPath(value.report_config.file_name2);
                value.report_config.header_logo2 = processImg(value.report_config.header_logo2, value.report_config.base_64_2);
            }

            if (!_repo.Edit(id, value)) return BadRequest(new { result = "Failed..." });


            return Ok(new { result = "Successful..." });
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            var result = _repo.Delete(id);

            if (!result) return BadRequest(new { response = "Delete Project unsuccessful" });

            return Ok(new { response = "Project deleted successfully" });
        }


        public string processPath(string filename)
        {
            var filePathName = "image" + "_" +
       DateTime.Now.ToString().Replace("/", "").Replace(":", "").Replace(" ", "") +
       Path.GetExtension(filename);
            var file_path = Path.Combine(Directory.GetCurrentDirectory(),
                "Upload", filePathName);

            return file_path;
        }

        public string processImg(string img, string file_path)
        {

            if (img.Contains(","))
            {
                img = img.Substring(img.IndexOf(",") + 1);
                //FileAsByteArray = Convert.FromBase64String(value.person_image);
            }

            if (System.IO.File.Exists(file_path))
            {
                System.IO.File.Delete(file_path);
            }

            using (var fs = new FileStream(file_path, FileMode.CreateNew))
            {
                byte[] info = System.Convert.FromBase64String(img);
                // Add some information to the file.
                fs.Write(info, 0, info.Length);
            }

            return img;
        }
    }
}
