using Heralabs.ClientManagement.Core.DTOs;

namespace Heralabs.ClientManagement.Core.Interfaces
{
    public interface IClientCacheService
    {
        Task<ClientCacheInfo?> GetClientInfoAsync(string clientCode);
        void InvalidateClientCache(string clientCode);
    }
}
