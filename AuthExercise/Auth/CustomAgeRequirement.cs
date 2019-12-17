using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExercise.Auth
{
    /// <summary>
    /// 自定义Requirement
    /// </summary>
    public class CustomAgeRequirement : IAuthorizationRequirement
    {
        //比较参数
        public CustomAgeRequirement(int age)
        {
            Age = age;
        }
        public int Age { get; }
    }

    //自定义RequirementHandler
    public class CustomAgeRequirementHandler
        : AuthorizationHandler<CustomAgeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, 
            CustomAgeRequirement requirement)
        {
            var ageClaim = context.User.Claims.FirstOrDefault(a => a.Type == "age");
            if(ageClaim == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            int defaultAge = 0;
            var isAge = int.TryParse(ageClaim.Value, out defaultAge);
            if (!isAge)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            if(defaultAge < requirement.Age)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }

    //自定义扩展方法
    public static class CustomRequirementExtensions
    {
        public static AuthorizationPolicyBuilder AddCustomAgeRequirement(
            this AuthorizationPolicyBuilder builder, int age)
        {
            builder.AddRequirements(new CustomAgeRequirement(age));
            return builder;
        }
    }
}
