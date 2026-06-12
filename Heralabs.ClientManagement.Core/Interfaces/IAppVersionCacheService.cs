using Heralabs.ClientManagement.Core.DTOs;

namespace Heralabs.ClientManagement.Core.Interfaces
{
    public interface IAppVersionCacheService
    {
        Task<WebApiVersionCacheInfo?> GetWebApiVersionAsync(string version);
        Task<MobileAppVersionCacheInfo?> GetSuggestedMobileVersionAsync(string platformString, long minRequiredVerOrder, long? maxAllowedVerOrder);
        void InvalidateWebApiVersionCache(string version);
        void InvalidateAppApiVersionCache();
    }
}
