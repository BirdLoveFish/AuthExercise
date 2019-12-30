using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcClient.Models;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Privacy()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var user = User.Claims;
            //accessToken有scope
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            //idToken没有scope
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            

            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize(Roles = "admin1")]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var response = await apiClient.GetAsync("http://localhost:5001/secret/index");

            var content = await response.Content.ReadAsStringAsync();
            await RefreshToken();
            return Ok(new
            {
                access_token = accessToken,
                message = content
            });
        }

        public async Task RefreshToken()
        {
            var serverClient = _httpClientFactory.CreateClient();

            var discovery = await serverClient
                .GetDiscoveryDocumentAsync("http://localhost:5000");

            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var refreshClient = _httpClientFactory.CreateClient();

            var tokenResponse = await refreshClient.RequestRefreshTokenAsync(
                new RefreshTokenRequest
            {
                Address = discovery.TokenEndpoint,
                RefreshToken = refreshToken,
                ClientId = "client_id_mvc",
                ClientSecret = "secret",
            });

            var authInfo = await HttpContext.AuthenticateAsync("Cookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            await HttpContext.SignInAsync("Cookie", authInfo.Principal,authInfo.Properties);
        }

        public IActionResult Denied()
        {
            return Ok("Denied");
        }
    }
}
