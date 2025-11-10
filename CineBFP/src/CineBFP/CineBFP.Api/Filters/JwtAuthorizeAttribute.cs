using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace CineBFP.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        public JwtAuthorizeAttribute(string roles = "")
        {
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
            Roles = roles; 
        }
    }
}
