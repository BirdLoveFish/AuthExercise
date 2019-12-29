using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace ApiTwo.Controllers
{
    [Route("[controller]/[action]")]
    public class HomeController: ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return Ok("Api Two Home Index");
        }

        public async Task<IActionResult> Value()
        {
            var serverClient = _httpClientFactory.CreateClient();

            var discovery = await serverClient
                .GetDiscoveryDocumentAsync("http://localhost:5000");

            var tokenRsponse = await serverClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = "client_id",
                ClientSecret = "secret",
                Scope = "ApiOne"
            });

            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(tokenRsponse.AccessToken);
            var response = await apiClient.GetAsync("http://localhost:5001/secret/index");

            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenRsponse.AccessToken,
                message = content
            });
        }




    }
}
