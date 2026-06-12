namespace Heralabs.ClientManagement.Domain.Common
{
    public abstract class BaseAuditableEntity
    {
        public DateTime Created { get; set; } = DateTime.UtcNow;
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? Updated { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
