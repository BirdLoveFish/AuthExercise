using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Route("[controller]")]
    public class IdentityController : Controller
    {
        [HttpGet("Authorize")]
        public IActionResult Authorize(
            string client_id,
            string scope,
            string response_type,
            string redirect_uri,
            string state)
        {
            var query = new QueryBuilder();
            query.Add("redirect_uri", redirect_uri);
            query.Add("state", state);
            return View(model:query.ToString());
        }

        [HttpPost("Authorize")]
        public IActionResult Authorize(
            string username,
            string state,
            string redirect_uri)
        {
            var query = new QueryBuilder();
            query.Add("code", "BABAABABABA");
            query.Add("state", state);
            return Redirect($"{redirect_uri}{query.ToString()}");
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token(
            string grant_type,
            string code,
            string redirect_uri,
            string client_id)
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
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: signingCredentials
                );
            //将token转为字符串，Base64编码
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var response = new
            {
                access_token,
                token_type = "Bearer",
                raw_claim = "raw_claim",
                expires_in="3600",
            };

            var responseStr = JsonConvert.SerializeObject(response);

            var responseByte = Encoding.UTF8.GetBytes(responseStr);

            await Response.Body.WriteAsync(responseByte, 0, responseByte.Length);
            //await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("sss"));

            return Redirect(redirect_uri);
        }
    }
}
