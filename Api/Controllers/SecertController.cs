using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("secret")]
    public class SecertController : ControllerBase
    {
        [Authorize]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok("api secret index");
        }
    }
}
