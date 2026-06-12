using Heralabs.ClientManagement.Domain.Enums;

namespace Heralabs.ClientManagement.Core.DTOs
{
    public class MobileAppVersionCacheInfo
    {
        public long VersionOrder { get; set; }
        public string Version { get; set; } = string.Empty;
        public string? DownloadPackageUrl { get; set; }
        public AppPlatform AppPlatform { get; set; }
    }
}
