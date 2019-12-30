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
        //if no user, visit IdentityServer
        [Authorize]
        public IActionResult Index()
        {
            var claims = User.Claims.ToList();
            return Ok(claims.First().Value);
        }
    }
}
