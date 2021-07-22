using AuthService.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AuthService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private IAuthenticationService _authenticateService;

        public TokenController(IAuthenticationService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public ActionResult GetToken([FromBody] PasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Request");

            var token = string.Empty;
            if (_authenticateService.IsAuthenticated(request, out token))
                return Ok(token);

            return BadRequest("Invalid Request");
        }
    }
}
