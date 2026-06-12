using Heralabs.ClientManagement.Domain.Common;
using Heralabs.ClientManagement.Domain.Enums;

namespace Heralabs.ClientManagement.Domain.Entities
{
    public class Client : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClientCode { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public string SecurityCode { get; set; } = string.Empty;
        public string ClientSecretKey { get; set; } = Guid.NewGuid().ToString("N");
        public string? Note { get; set; }
        public ClientStatus Status { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
