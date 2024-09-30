using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HRSystem.WebAPI.Controllers
{
    /// <summary>
    /// instead of injecting meditor object in every controller we will inject it here
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        private ISender _sender;

        public ISender Sender => _sender ?? HttpContext.RequestServices.GetService<ISender>()!;
    }
}
