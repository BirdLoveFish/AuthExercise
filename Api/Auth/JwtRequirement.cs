using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Auth
{
    public class JwtRequirement: IAuthorizationRequirement
    {
    }

    public class JwtRequirementHandler : AuthorizationHandler<JwtRequirement>
    {
        private readonly HttpContext _httpContext;
        private readonly HttpClient _httpClient;

        public JwtRequirementHandler(
            IHttpContextAccessor httpContextAccessor,
            IHttpClientFactory httpClientFactory)
        {
            _httpContext = httpContextAccessor.HttpContext;
            _httpClient = httpClientFactory.CreateClient();
        }
        protected async override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            JwtRequirement requirement)
        {
            if(_httpContext.Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var accessToken = authHeader.ToString().Split(' ')[1];

                var response = await _httpClient.GetAsync($"http://localhost:55580/identity/validate?access_token={accessToken}");
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    context.Succeed(requirement);
                }
            }
            await Task.CompletedTask;
        }
    }
}
