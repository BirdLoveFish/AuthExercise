/*
 *  基于资源的授权
 */
using AuthExercise.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExercise.Controllers
{
    [Route("[controller]/[action]")]
    public class ResourceController : ControllerBase
    {
        private readonly IAuthorizationService _authorizationService;

        public ResourceController(IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public async Task<IActionResult> Get(string operation)
        {
            var document = new Document()
            {
                Author = "zhang",
                Secret = "secret",
            };

            var requirement = new OperationAuthorizationRequirement
            {
                Name = operation,
            };
            var authorizationResult = await _authorizationService
                .AuthorizeAsync(User, document, requirement);

            return Ok(authorizationResult);
        }

    }
}
