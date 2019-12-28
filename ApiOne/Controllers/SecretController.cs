using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiOne.Controllers
{
    [Route("[controller]/[action]")]
    public class SecretController:ControllerBase
    {
        [Authorize]
        public IActionResult Index()
        {
            return Ok("Api One Secret Index");
        }
    }
}
