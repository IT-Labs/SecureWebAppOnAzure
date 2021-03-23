using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AzureSecurityDemo.Helpers
{
    public class AdminRequirement : AuthorizationHandler<AdminRequirement>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AdminRequirement requirement)
        {
            if (context.User.HasClaim(ClaimTypes.Role, "Administrator"))
            {
                context.Succeed(requirement);
            }
            return Task.FromResult(0);
        }
    }
}
