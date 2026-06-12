using Heralabs.ClientManagement.Core.DTOs;
using Heralabs.ClientManagement.Core.Helpers;
using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Constants;
using Heralabs.ClientManagement.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Heralabs.ClientManagement.Core.Services
{
    public class AppVersionCacheService : IAppVersionCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IMobileAppVersionRepository _mobileAppVersionRepository;
        private readonly IWebApiVersionRepository _webApiVersionRepository;

        public AppVersionCacheService(IMemoryCache cache, IMobileAppVersionRepository mobileAppVersionRepository, IWebApiVersionRepository webApiVersionRepository)
        {
            _cache = cache;
            _mobileAppVersionRepository = mobileAppVersionRepository;
            _webApiVersionRepository = webApiVersionRepository;
        }

        public async Task<WebApiVersionCacheInfo?> GetWebApiVersionAsync(string version)
        {
            string cacheKey = string.Format(SystemContants.WebApiVersionCacheTemplate, version);
            
            if (!_cache.TryGetValue(cacheKey, out WebApiVersionCacheInfo? versionInfo))
            {
                var entity = await _webApiVersionRepository.GetByVersionCodeAsync(version);
                if (entity != null)
                {
                    versionInfo = new WebApiVersionCacheInfo
                    {
                        Version = entity.Version,
                        IsActive = entity.IsActive,
                        MinimumMobileAppVersion = entity.MinimumMobileAppVersion,
                        MaximumMobileAppVersion = entity.MaximumMobileAppVersion
                    };

                    _cache.Set(cacheKey, versionInfo, TimeSpan.FromDays(SystemContants.AppVersionCacheTimeOutDays));
                }
            }
            return versionInfo;
        }

        private async Task<List<MobileAppVersionCacheInfo>> GetAllActiveMobileVersionsAsync()
        {
            if (!_cache.TryGetValue(SystemContants.AllMobileVersionsCacheKey, out List<MobileAppVersionCacheInfo>? activeVersions) || activeVersions == null)
            {
                var entities = await _mobileAppVersionRepository.GetAllActivepVersionsAsync();
                activeVersions = entities.Select(e => new MobileAppVersionCacheInfo
                {
                    VersionOrder = VersionHelper.CalculateVersionOrder(e.Version),
                    Version = e.Version,
                    DownloadPackageUrl = e.DownloadPackageUrl,
                    AppPlatform = e.AppPlatform
                }).ToList();

                _cache.Set(SystemContants.AllMobileVersionsCacheKey, activeVersions, TimeSpan.FromDays(SystemContants.AppVersionCacheTimeOutDays));
            }

            return activeVersions;
        }

        public async Task<MobileAppVersionCacheInfo?> GetSuggestedMobileVersionAsync(
            string platformString,
            long minRequiredVerOrder,
            long? maxAllowedVerOrder) // maxAllowed lấy từ WebApiVersion.MaximumMobileAppVersion
        {
            if (!Enum.TryParse<AppPlatform>(platformString, true, out var platform))
                return null;

            var allVersions = await GetAllActiveMobileVersionsAsync();

            // Lọc theo Platform và >= Minimum
            var query = allVersions.Where(v => v.AppPlatform == platform && v.VersionOrder >= minRequiredVerOrder);

            // Lọc <= Maximum (Nếu API có quy định giới hạn trần)
            if (maxAllowedVerOrder.HasValue && maxAllowedVerOrder.Value > 0)
            {
                query = query.Where(v => v.VersionOrder <= maxAllowedVerOrder.Value);
            }

            // Lấy Version lớn nhất thỏa mãn vùng điều kiện
            return query.OrderByDescending(v => v.VersionOrder).FirstOrDefault();
        }

        public void InvalidateWebApiVersionCache(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                return;
            }
            string cacheKey = string.Format(SystemContants.WebApiVersionCacheTemplate, version);
            _cache.Remove(cacheKey);
        }

        public void InvalidateAppApiVersionCache()
        {
            _cache.Remove(SystemContants.AllMobileVersionsCacheKey);
        }
    }
}
