namespace Heralabs.ClientManagement.Domain.Entities
{
    public class DeviceTracking
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ClientCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Os { get; set; } = string.Empty;

        public string? AppVersion { get; set; }
        public string? DeviceBrand { get; set; }
        public string? DeviceModel { get; set; }
        public string? DeviceId { get; set; }

        public DateTime? LoggedAt { get; set; } = DateTime.UtcNow;
    }
}
