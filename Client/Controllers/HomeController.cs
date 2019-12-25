using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    //[Route("[controller]/[action]")]
    public class HomeController:ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("client index");
        }

        //没有验证的用户会去授权服务器验证
        [Authorize]
        public IActionResult Secret()
        {
            return Ok("client secret");
        }

        public async Task<IActionResult> Text()
        {
            string response = "text";
            var responseBytes = Encoding.UTF8.GetBytes(response);
            await Response.Body.WriteAsync(responseBytes,0, responseBytes.Length);
            return Ok("client index");
        }
    }
}
