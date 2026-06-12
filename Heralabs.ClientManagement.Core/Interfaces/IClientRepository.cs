using Heralabs.ClientManagement.Core.DTOs;
using Heralabs.ClientManagement.Domain.Entities;

namespace Heralabs.ClientManagement.Core.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> GetClientByCodeAsync(string clientCode);
        Task AddTrackingAsync(DeviceTracking tracking);
        Task SaveChangesAsync();
        Task<ClientCacheInfo?> GetClientCacheInfoAsync(string clientCode);
    }
}
