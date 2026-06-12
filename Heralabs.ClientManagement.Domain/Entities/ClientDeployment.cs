using Heralabs.ClientManagement.Domain.Common;

namespace Heralabs.ClientManagement.Domain.Entities
{
    public class ClientDeployment : BaseAuditableEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ClientId { get; set; }
        public string? EnvironmentName { get; set; }

        // API Container
        public string? ApiContainerName { get; set; }
        public int? ApiContainerMappedPort { get; set; }
        public string? WebApiVersionCode { get; set; } // Map logic với WebApiVersion.Version
        public string? ApiUrl { get; set; }

        // Web App Container
        public string? WebContainerName { get; set; }
        public int? WebContainerMappedPort { get; set; }
        public string? WebVersion { get; set; }
        public string? WebUrl { get; set; }

        // SQL Server Container
        public string? SqlContainerName { get; set; }
        public int? SqlMappedPort { get; set; }
        public string? SqlHeralabsDbName { get; set; }
        public string? SqlHeralabsLogDbName { get; set; }
        public string? SqlHeralabsDbUser { get; set; }
        public string? SqlHeralabsDbPasswordEncrypted { get; set; }

        // Postgres Container
        public string? PostgresContainerName { get; set; }
        public int? PostgresMappedPort { get; set; }
        public string? PostgresHeralabsDbName { get; set; }
        public string? PostgresHeralabsDbUser { get; set; }
        public string? PostgresHeralabsDbPasswordEncrypted { get; set; }

        public bool IsActive { get; set; }
    }
}
