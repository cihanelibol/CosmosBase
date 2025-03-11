using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace CosmosBase
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IMediator mediator;
        protected IMediator Mediator
        {
            get
            {
                return mediator ??= HttpContext.RequestServices.GetService<IMediator>();
            }
        }

        protected string? GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
        }

        protected int GetUserIdFromRequest()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            return Convert.ToInt32(userId);
        }
    }
}