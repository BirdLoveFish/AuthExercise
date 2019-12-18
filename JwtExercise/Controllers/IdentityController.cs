using JwtExercise.Extensions;
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

namespace JwtExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class IdentityController: ControllerBase
    {
        public IActionResult Home()
        {
            return Ok("Home");
        }

        [Authorize]
        public IActionResult Secret()
        {
            var identity = User.Identity;
            return Ok("Secret");
        }

        public IActionResult Authenticate()
        {
            //定义claims
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
            var tokenJson = new JwtSecurityTokenHandler().WriteToken(token);
            return Ok(new { access_token = tokenJson });
        }
    }
}
