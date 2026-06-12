namespace Heralabs.ClientManagement.Core.DTOs
{
    public class ClientInfoDto
    {
        public int StatusCode { get; set; }
        public string ApiUrl { get; set; } = string.Empty;
        public string ClientHmacSecretKey { get; set; } = string.Empty;
        public bool AppUpgradeRequired { get; set; } = false;
        public string AppUpgradeVersion { get; set; } = string.Empty;
        public string AppUpgradePackageUrl { get; set; } = string.Empty;
    }
}
