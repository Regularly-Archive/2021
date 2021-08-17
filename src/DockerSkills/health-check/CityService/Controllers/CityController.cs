using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CityService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CityController : ControllerBase
    {
        private static readonly string[] Cities = new[]
        {
            "中卫", "西安", "苏州", "安庆", "洛阳", "银川", "兰州"
        };

        private readonly ILogger<CityController> _logger;

        public CityController(ILogger<CityController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public dynamic Get()
        {
            var rnd = new Random();
            var city =  Cities[rnd.Next(Cities.Length)];
            return new { City = city, Now = DateTime.Now };
        }
    }
}
