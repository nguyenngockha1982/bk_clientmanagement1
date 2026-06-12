using Heralabs.ClientManagement.Api.Filters;
using Heralabs.ClientManagement.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Heralabs.ClientManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppVersionsController : ControllerBase
    {
        private readonly IAppVersionCacheService _appVersionCacheService;

        public AppVersionsController(IAppVersionCacheService appVersionCacheService)
        {
            _appVersionCacheService = appVersionCacheService;
        }

        // 1. Endpoint xóa cache của một Web API Version cụ thể
        [HttpPost("webapi/{version}/invalidate-cache")]
        [HmacAuthorization(UseCommonKey = true)] // Bảo vệ bằng Common Key (hoặc Admin Key)
        public IActionResult InvalidateWebApiVersionCache(string version)
        {
            _appVersionCacheService.InvalidateWebApiVersionCache(version);

            return Ok(new { Message = $"Cache for WebApi version '{version}' has been successfully invalidated." });
        }

        // 2. Endpoint xóa toàn bộ cache của Mobile App Versions
        [HttpPost("mobile/invalidate-cache")]
        [HmacAuthorization(UseCommonKey = true)]
        public IActionResult InvalidateAppApiVersionCache()
        {
            _appVersionCacheService.InvalidateAppApiVersionCache();

            return Ok(new { Message = "All Mobile App versions cache has been successfully invalidated." });
        }
    }
}
