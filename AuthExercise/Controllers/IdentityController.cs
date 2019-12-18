/*
 *  HttpContext.SignInAsync 手动登陆
 *  基本的Claim，Role，Policy授权认证
 */

using AuthExercise.Model;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class IdentityController : ControllerBase
    {
        public async Task<IActionResult> Authenticate(
            string name="zhang", string email= "531047332@qq.com",
            string role="Admin", string age="18")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                //添加角色
                new Claim(ClaimTypes.Role, role),
                //可以为数据添加类型
                new Claim("age", age, ClaimValueTypes.Integer)
            };
            //必须指定AuthenticationType，否则无法使用SignInAsync
            var claimsIdentity = new ClaimsIdentity(claims, "myClaims");
            //这种方式只是单纯的给User赋值，但是并不会返回Cookie，无法认证通过
            //HttpContext.User = new ClaimsPrincipal(claimsIdentity);
            //正确的授权方式
            var user = new ClaimsPrincipal(new[] { claimsIdentity });
            await HttpContext.SignInAsync(user);
            return Ok("Authenticate");
        }

        [Authorize]
        public IActionResult Login()
        {
            return Ok("login success");
        }

        [Authorize]
        public IActionResult Secret()
        {
            return Ok("secret");
        }

        //策略 claim授权
        [Authorize(Policy = "needName")]
        public IActionResult AuthNeedName()
        {
            return Ok("AuthNeedName");
        }

        //策略 角色授权
        [Authorize(Policy = "policyAdmin")]
        public IActionResult AuthAdmin1()
        {
            return Ok("AuthAdmin1");
        }

        //直接 角色授权
        [Authorize(Roles = "Admin")]
        public IActionResult AuthAdmin2()
        {
            return Ok("AuthAdmin2");
        }
        //自定义策略授权
        [Authorize(Policy = "customAgePolicy")]
        public IActionResult AuthAge()
        {
            return Ok("AuthAge");
        }

    }
}
