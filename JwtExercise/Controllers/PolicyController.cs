using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JwtExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class PolicyController: ControllerBase
    {
        [Authorize(Policy = "gender")]
        public IActionResult Gender()
        {
            return Ok("gender = 1|2 success");
        }

        [Authorize(Policy = "needAge")]
        public IActionResult Age()
        {
            return Ok("needAge");
        }
    }
}
