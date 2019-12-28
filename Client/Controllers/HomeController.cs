using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Client.Controllers
{
    //[Route("[controller]/[action]")]
    public class HomeController:ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return Ok("client index");
        }

        //没有验证的用户会去授权服务器验证
        [Authorize]
        public async Task<IActionResult> Secret()
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var serverReponse = await client.GetAsync("http://localhost:55580/secret/index");
            var serverContent = await serverReponse.Content.ReadAsStringAsync();

            //var apiReponse = await client.GetAsync("http://localhost:6616/secret/index");
            //var apiContent = await apiReponse.Content.ReadAsStringAsync();
            
            return Ok();
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
