using Heralabs.ClientManagement.Domain.Common;

namespace Heralabs.ClientManagement.Domain.Entities
{
    public class WebApiVersion : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Version { get; set; } = string.Empty;
        public string MinimumMobileAppVersion { get; set; } = string.Empty;
        public string? MaximumMobileAppVersion { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? ReleaseNote { get; set; }
        public bool IsActive { get; set; }
    }
}
