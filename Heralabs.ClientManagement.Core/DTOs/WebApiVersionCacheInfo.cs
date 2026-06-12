namespace Heralabs.ClientManagement.Core.DTOs
{
    public class WebApiVersionCacheInfo
    {
        public string Version { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string MinimumMobileAppVersion { get; set; } = string.Empty;
        public string? MaximumMobileAppVersion { get; set; }
    }
}
