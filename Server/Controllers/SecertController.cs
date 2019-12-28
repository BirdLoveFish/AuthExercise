using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("secret")]
    public class SecertController:ControllerBase
    {
        [Authorize]
        [HttpGet("index")]
        public IActionResult Index()
        {
            return Ok("server secret index");
        }
    }
}
