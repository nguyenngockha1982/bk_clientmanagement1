using Heralabs.ClientManagement.Core.DTOs;
using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace Heralabs.ClientManagement.Core.Services
{
    public class ClientCacheService : IClientCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly IClientRepository _repository;

        public ClientCacheService(IMemoryCache cache, IClientRepository repository)
        {
            _cache = cache;
            _repository = repository;
        }

        public async Task<ClientCacheInfo?> GetClientInfoAsync(string clientCode)
        {
            if (string.IsNullOrWhiteSpace(clientCode))
            {
                return null;
            }

            var normalizedClientCode = clientCode.Trim().ToUpper();
            string cacheKey = string.Format(SystemContants.ClientCacheKeyTemplate, normalizedClientCode);

            if (!_cache.TryGetValue(cacheKey, out ClientCacheInfo? clientInfo))
            {
                // Cache miss -> Gọi Repository
                clientInfo = await _repository.GetClientCacheInfoAsync(normalizedClientCode);

                if (clientInfo != null)
                {
                    var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(TimeSpan.FromDays(SystemContants.ClientCacheTimeOutDays));

                    _cache.Set(cacheKey, clientInfo, cacheEntryOptions);
                }
            }

            return clientInfo;
        }

        public void InvalidateClientCache(string clientCode)
        {
            if (string.IsNullOrWhiteSpace(clientCode))
            {
                return;
            }
            var normalizedClientCode = clientCode.Trim().ToUpper();
            string cacheKey = string.Format(SystemContants.ClientCacheKeyTemplate, normalizedClientCode);
            _cache.Remove(cacheKey);
        }
    }
}
