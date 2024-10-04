using HRSystem.Common.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace HRSystem.WebAPI.Attributes
{
    public class MustHavePermessionAttribute : AuthorizeAttribute
    {
        public MustHavePermessionAttribute(string feature , string action)
           => Policy = AppPermession.NameFor(feature, action);

    }
}
