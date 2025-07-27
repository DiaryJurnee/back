using API.Modules.Filters;
using Microsoft.AspNetCore.Mvc;

namespace API.Attributes;

public class CustomAuthorizeAttribute : TypeFilterAttribute
{
    public CustomAuthorizeAttribute(string role) : base(typeof(AuthorizationFilter))
    {
        Arguments = [role];
    }
}
