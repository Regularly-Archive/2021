using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GrpcJwt.Handlers
{
    public class UserRoleRequirement : 
        AuthorizationHandler<UserRoleRequirement, HubInvocationContext>, IAuthorizationRequirement
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, UserRoleRequirement requirement, HubInvocationContext resource)
        {
            var userRole = context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value;
            if (userRole == "Admin" && resource.HubMethodName == "Echo")
                context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
