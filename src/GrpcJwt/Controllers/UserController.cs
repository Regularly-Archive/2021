using GrpcJwt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GrpcJwt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly IAuthenticateService _authenticateService;
        public UserController(IAuthenticateService authenticateService)
        {
            _authenticateService = authenticateService;
        }

        [HttpPost]
        [Route("Token")]
        public IActionResult Token([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            if (_authenticateService.IsAuthenticated(request, out string token))
                return Ok(new { access_token = token });

            return BadRequest();
        }
    }
}
