/*
 *  Cliams转换程序
 *  会在授权处理程序之前运行
 */

using Microsoft.AspNetCore.Authentication;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthExercise.Auth
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (!principal.Claims.Any(a => a.Type == "Hello"))
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(
                    new Claim("Hello", "world"));
            }
            return Task.FromResult(principal);
        }
    }
}
