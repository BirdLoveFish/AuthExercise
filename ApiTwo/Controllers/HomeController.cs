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
        //HttpClient DI
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return Ok("Api Two Home Index");
        }

        //visit protected api in ApiOne
        public async Task<IActionResult> Value()
        {
            //create HttpClient
            var serverClient = _httpClientFactory.CreateClient();
            //get IdentityServer Discovery Document about AuthorizationEndpoint
            //TokenEndpoint and UserInfoEndpoint
            var discovery = await serverClient
                .GetDiscoveryDocumentAsync("http://localhost:5000");
            //http request and get response
            var tokenRsponse = await serverClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = discovery.TokenEndpoint,
                ClientId = "client_id",
                ClientSecret = "secret",
                Scope = "ApiOne"
            });

            var apiClient = _httpClientFactory.CreateClient();
            //set header of "Bearer xxx.yyy.zzz"
            apiClient.SetBearerToken(tokenRsponse.AccessToken);
            //visit website
            var response = await apiClient.GetAsync("http://localhost:5001/secret/index");
            //get result
            var content = await response.Content.ReadAsStringAsync();

            return Ok(new
            {
                access_token = tokenRsponse.AccessToken,
                message = content
            });
        }




    }
}
