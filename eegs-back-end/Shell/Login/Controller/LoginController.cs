using eegs_back_end.Shell.Login.Model;
using eegs_back_end.Shell.Login.Repository;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace eegs_back_end.Shell.Login.Controller
{
    [Route("login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private ILoginRepository _loginRepo;
        private IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _config;
        public LoginController(ILoginRepository loginRepo, IHttpContextAccessor httpContextAccessor, IConfiguration config)
        {
            _loginRepo = loginRepo;
            _httpContextAccessor = httpContextAccessor;
            _config = config;
        }

        // GET: api/<LoginController>
        [HttpPost]
        public object Post(LoginRequest value)
        {
            var result = _loginRepo.Login(value);

            if (result == null) return null ;

            return result;
        }


    }
}
