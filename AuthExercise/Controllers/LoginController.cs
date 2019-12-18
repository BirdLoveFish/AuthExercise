/*
 *  使用asp identity来登陆
 *  UserManager SignInManager RoleManager
 */
using AuthExercise.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class LoginController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public LoginController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 注册不会返回cookie
        /// </summary>
        /// <param name="applicationUser"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(
            [FromBody]ApplicationUser applicationUser, string password)
        {
            var identityResult = await _userManager.CreateAsync(applicationUser, password);
            if (identityResult.Succeeded)
            {
                return Ok("success");
            }
            return BadRequest(identityResult.Errors);
        }

        /// <summary>
        /// 登陆会返回cookie，在AddIdentity的情况下，
        /// 只有在这里登陆过才能成功被认证授权
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Login(
            [FromForm]string username, [FromForm]string password)
        {
            var signInResult = await _signInManager
                .PasswordSignInAsync(username,password,false,false);
            if (signInResult.Succeeded)
            {
                return Ok("success");
            }
            return BadRequest(signInResult);
        }

        /// <summary>
        /// 登出就是清除cookie
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok("Logout");
        }
    }
}
