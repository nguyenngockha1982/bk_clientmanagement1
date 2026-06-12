using Heralabs.ClientManagement.Domain.Common;
using Heralabs.ClientManagement.Domain.Enums;

namespace Heralabs.ClientManagement.Domain.Entities
{
    public class MobileAppVersion : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public AppPlatform AppPlatform { get; set; }
        public string Version { get; set; } = string.Empty;
        public string? DownloadPackageUrl { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string? ReleaseNote { get; set; }
        public bool IsActive { get; set; }
    }
}
