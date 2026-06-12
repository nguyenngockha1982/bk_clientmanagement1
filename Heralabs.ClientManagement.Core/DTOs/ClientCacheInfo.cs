using Heralabs.ClientManagement.Domain.Enums;

namespace Heralabs.ClientManagement.Core.DTOs
{
    public class ClientCacheInfo
    {
        public Guid Id { get; set; }
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string ClientSecretKey { get; set; } = string.Empty;
        public ClientStatus Status { get; set; }
        public string? WebApiVersionCode { get; set; }
        public string? ApiUrl { get; set; }
    }
}
