/*
 *  自定义策略提供程序
 *  该方法是应用于大范围的数据策略
 *  通过action的属性来传参
 *  每一个不同的参数就是一种新策略
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthExercise.Auth
{
    //定义requirement
    public class MinAgeRequirement : IAuthorizationRequirement
    {
        public MinAgeRequirement(int age)
        {
            Age = age;
        }
        public int Age { get; }
    }

    //定义RequirementHandler
    public class MinAgeRequirementHandler
        : AuthorizationHandler<MinAgeRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            MinAgeRequirement requirement)
        {
            var ageClaim = context.User.Claims.FirstOrDefault(a => a.Type == "age");
            if (ageClaim == null)
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
            if (defaultAge < requirement.Age)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            context.Succeed(requirement);
            return Task.CompletedTask;
        }
    }
    //定义Attribute，给policy赋值，这是最关键的，相当于每不同的age都新定义了一个策略
    public class MinimumAgeAuthorizeAttribute : AuthorizeAttribute
    {
        const string POLICY_PREFIX = "MinimumAge";
        public MinimumAgeAuthorizeAttribute(int age)
        {
            Age = age;
        }

        public int Age
        {
            get
            {
                if (int.TryParse(Policy.Substring(POLICY_PREFIX.Length), out var age))
                {
                    return age;
                }
                return default;
            }
            set
            {
                Policy = $"{POLICY_PREFIX}{value.ToString()}";
            }
        }
    }

    //可能由于MinimumAgeAuthorize没有默认的policy，则进入到该方法
    public class MinimumAgePolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "MinimumAge";
        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }

        public MinimumAgePolicyProvider(IOptions<AuthorizationOptions> options)
        {
           FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        {
            return FallbackPolicyProvider.GetDefaultPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetFallbackPolicyAsync()
        {
            return FallbackPolicyProvider.GetFallbackPolicyAsync();
        }

        public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
                int.TryParse(policyName.Substring(POLICY_PREFIX.Length), out var age))
            {
                var policy = new AuthorizationPolicyBuilder();
                //这个age是外界传进来的，更加灵活
                policy.AddRequirements(new MinAgeRequirement(age));
                return Task.FromResult(policy.Build());
            }
            return FallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
