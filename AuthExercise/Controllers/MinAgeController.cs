using AuthExercise.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class MinAgeController:ControllerBase
    {
        [MinimumAgeAuthorize(60)]
        public IActionResult Get60()
        {
            return Ok("Get 60");
        }

        [MinimumAgeAuthorize(50)]
        public IActionResult Get50()
        {
            return Ok("Get 50");
        }
    }
}
