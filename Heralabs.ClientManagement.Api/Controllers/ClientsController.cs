using Heralabs.ClientManagement.Api.Extensions;
using Heralabs.ClientManagement.Api.Filters;
using Heralabs.ClientManagement.Core.DTOs;
using Heralabs.ClientManagement.Core.Helpers;
using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Entities;
using Heralabs.ClientManagement.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Heralabs.ClientManagement.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ILogger<ClientsController> _logger;
        private readonly IClientRepository _clientRepository;
        private readonly IClientCacheService _clientCacheService;
        private readonly IAppVersionCacheService _appVersionCacheService;
        

        public ClientsController(ILogger<ClientsController> logger, IClientRepository clientRepository, IClientCacheService clientCacheService, IAppVersionCacheService appVersionCacheService)
        {
            _logger = logger;
            _clientRepository = clientRepository;
            _clientCacheService = clientCacheService;
            _appVersionCacheService = appVersionCacheService;
        }

        [HttpPost("connect")]
        [HmacAuthorization(UseCommonKey = true)]
        public async Task<IActionResult> GetClientInfo([FromBody] ClientConnectionDto dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.ClientCode) ||
                    dto.SecurityCodeRequired && string.IsNullOrWhiteSpace(dto.SecurityCode) ||
                    string.IsNullOrWhiteSpace(dto.MobileAppVersion) ||
                    string.IsNullOrWhiteSpace(dto.MobilePlatform))
                {
                    return BadRequest("ClientCode, SecurityCode, MobileAppVersion, and MobilePlatform could not be null or empty.");
                }

                var clientInfo = await _clientCacheService.GetClientInfoAsync(dto.ClientCode);
                if (clientInfo == null)
                {
                    var notFoundResponse = new ClientInfoDto
                    {
                        StatusCode = (int)ClientStatus.NotFound,
                    };
                    return Ok(notFoundResponse);
                }

                if (dto.SecurityCodeRequired && clientInfo.SecurityCode != dto.SecurityCode)
                {
                    var notFoundResponse = new ClientInfoDto
                    {
                        StatusCode = (int)ClientStatus.WrongSecurityCode,
                    };
                    return Ok(notFoundResponse);
                }

                var mobileAppUpgradeRequired = false;
                var mobileAppUpgradeVersion = string.Empty;
                var mobileAppUpgradePackageUrl = string.Empty;

                if (clientInfo.Status == Domain.Enums.ClientStatus.Active)
                {
                    if (!string.IsNullOrEmpty(dto.MobileAppVersion) &&
                        !string.IsNullOrEmpty(dto.MobilePlatform) &&
                        !string.IsNullOrEmpty(clientInfo.WebApiVersionCode))
                    {
                        var apiVersionInfo = await _appVersionCacheService.GetWebApiVersionAsync(clientInfo.WebApiVersionCode);

                        if (apiVersionInfo != null)
                        {
                            long currentAppVerOrder = VersionHelper.CalculateVersionOrder(dto.MobileAppVersion);
                            long minRequiredVerOrder = VersionHelper.CalculateVersionOrder(apiVersionInfo.MinimumMobileAppVersion);

                            // Lấy giới hạn trần từ API (nếu API có set MaximumMobileAppVersion)
                            long maxAllowedVerOrder = VersionHelper.CalculateVersionOrder(apiVersionInfo.MaximumMobileAppVersion);

                            if (currentAppVerOrder > 0)
                            {
                                // 1. Kiểm tra Force Update
                                if (currentAppVerOrder < minRequiredVerOrder)
                                {
                                    mobileAppUpgradeRequired = true;
                                }

                                // 2. Tìm Suggestion Version an toàn nhất cho API này
                                var suggestedMobileInfo = await _appVersionCacheService.GetSuggestedMobileVersionAsync(
                                    dto.MobilePlatform,
                                    minRequiredVerOrder,
                                    maxAllowedVerOrder > 0 ? maxAllowedVerOrder : null);

                                // 3. Nếu tìm thấy bản tốt hơn bản hiện tại, khuyến nghị nâng cấp
                                if (suggestedMobileInfo != null && currentAppVerOrder < suggestedMobileInfo.VersionOrder)
                                {
                                    mobileAppUpgradeVersion = suggestedMobileInfo.Version;
                                    mobileAppUpgradePackageUrl = suggestedMobileInfo.DownloadPackageUrl ?? string.Empty;
                                }
                            }
                            else
                            {
                                _logger.LogWarning($"Invalid version format passed. ClientVer: {dto.MobileAppVersion}");
                            }
                        }
                    }
                }

                var response = new ClientInfoDto
                {
                    StatusCode = (int)clientInfo.Status,
                    ApiUrl = clientInfo.ApiUrl ?? "",
                    ClientHmacSecretKey = clientInfo.ClientSecretKey ?? string.Empty,
                    AppUpgradeRequired = mobileAppUpgradeRequired,
                    AppUpgradeVersion = mobileAppUpgradeVersion,
                    AppUpgradePackageUrl = mobileAppUpgradePackageUrl
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"GetClientInfo '{dto.ClientCode}' Exception: {ex.Message}", ex);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPost("{clientCode}/tracking")]
        [HmacAuthorization(UseCommonKey = false)]
        public async Task<IActionResult> TrackDevice(string clientCode, [FromBody] DeviceTrackingDto dto)
        {
            try
            {
                var client = await _clientCacheService.GetClientInfoAsync(clientCode);
                if (client == null)
                    return NotFound("Client not found.");

                var tracking = new DeviceTracking
                {
                    ClientCode = clientCode,
                    Os = dto.Os,
                    AppVersion = dto.AppVersion,
                    DeviceBrand = dto.DeviceBrand,
                    DeviceModel = dto.DeviceModel,
                    DeviceId = dto.DeviceId,
                    Username = dto.Username,
                    LoggedAt = DateTime.UtcNow
                };

                await _clientRepository.AddTrackingAsync(tracking);
                await _clientRepository.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError($"TrackDevice '{clientCode}' Exception", ex);
                return this.InternalServerError(ex);
            }
        }

        [HttpPost("{clientCode}/invalidate-cache")]
        [HmacAuthorization(UseCommonKey = true)]
        public IActionResult InvalidateClientCache(string clientCode)
        {
            try
            {

                _clientCacheService.InvalidateClientCache(clientCode);
                return Ok(new { Message = $"Cache for client '{clientCode}' has been successfully invalidated." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"TrackDevice '{clientCode}' Exception", ex);
                return this.InternalServerError(ex);
            }
        }
    }
}
