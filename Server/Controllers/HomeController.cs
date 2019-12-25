using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController: ControllerBase
    {
        public IActionResult Index()
        {
            return Ok("server index");
        }

        [Authorize]
        public IActionResult Secret()
        {
            return Ok("server secret");
        }

        public IActionResult Authenticate()
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "1"),
                new Claim(JwtRegisteredClaimNames.Email, "531047332@qq.com"),
                new Claim(JwtRegisteredClaimNames.Gender, "1"),
                new Claim("age", "18"),
            };

            //定义秘钥
            var securityKey = new SymmetricSecurityKey
                (Encoding.UTF8.GetBytes(JwtInformation.SecurityKey));
            //签名
            var signingCredentials =
                new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //定义token
            var token = new JwtSecurityToken(
                issuer: JwtInformation.Issure,
                audience: JwtInformation.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: signingCredentials
                );
            //将token转为字符串，Base64编码
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(access_token);
        }
    }
}
