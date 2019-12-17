using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class AuthServiceController : ControllerBase
    {
        //也可以将IAuthorizationService直接在action中注入
        private readonly IAuthorizationService _authorizationService;
        public AuthServiceController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        /// <summary>
        /// AddIdentity之后这个也会失效
        /// 因为Identity/Authenticate返回的Cookie的名字改变了，所以无法解析Cookie
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Auth()
        {
            var policyBuilder = new AuthorizationPolicyBuilder();
            policyBuilder.RequireClaim(ClaimTypes.Name);
            var policy = policyBuilder.Build();
            var result = await _authorizationService.AuthorizeAsync(User, policy);
            if (result.Succeeded)
            {
                return true;
            }
            return false;
            //也可以使用重定向，直接让api暴露给外面
        }
        //将服务直接注入到action
        //public async Task<IActionResult> Auth(
        //    IAuthorizationService authorizationService)
        //{

        //}

        public async Task<IActionResult> Login()
        {
            if(await Auth())
                return Ok("login");
            return BadRequest("fail");
        }

        public async Task<IActionResult> Secret()
        {
            if (await Auth())
                return Ok("Secret");
            return BadRequest("fail");
        }
    }
}
