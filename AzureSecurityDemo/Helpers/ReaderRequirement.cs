using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureSecurityDemo.Helpers
{
    public class ReaderRequirement : AuthorizationHandler<ReaderRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ReaderRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, "Reader"))
            {
                context.Succeed(requirement);
            }
            return Task.FromResult(0);
        }
    }
}
