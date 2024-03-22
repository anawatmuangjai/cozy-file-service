using CozyFileService.Application.Contracts;
using System.Security.Claims;

namespace CozyFileService.WebApi.Services
{
    public class LoggedInUserService : ILoggedInUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public LoggedInUserService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public string UserId => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public string UserName => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

        public string Email => _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
    }
}
