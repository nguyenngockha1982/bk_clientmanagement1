namespace Heralabs.ClientManagement.Core.DTOs
{
    public class ClientConnectionDto
    {
        public string ClientCode { get; set; } = string.Empty;
        public string SecurityCode { get; set; } = string.Empty;
        public bool SecurityCodeRequired { get; set; }
        public string MobileAppVersion { get; set; } = string.Empty;
        public string MobilePlatform { get; set; } = string.Empty;
    }
}
