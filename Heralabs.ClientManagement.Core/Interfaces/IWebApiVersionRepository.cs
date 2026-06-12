using Heralabs.ClientManagement.Domain.Entities;

namespace Heralabs.ClientManagement.Core.Interfaces
{
    public interface IWebApiVersionRepository
    {
        Task<WebApiVersion?> GetByVersionCodeAsync(string version);
    }
}
