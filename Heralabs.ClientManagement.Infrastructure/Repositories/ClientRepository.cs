using Heralabs.ClientManagement.Core.DTOs;
using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Entities;
using Heralabs.ClientManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Heralabs.ClientManagement.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly AppDbContext _context;
        public ClientRepository(AppDbContext context) => _context = context;

        public async Task<Client?> GetClientByCodeAsync(string clientCode)
            => await _context.Clients.FirstOrDefaultAsync(c => c.ClientCode == clientCode);

        public async Task AddTrackingAsync(DeviceTracking tracking)
            => await _context.DeviceTrackings.AddAsync(tracking);

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<ClientCacheInfo?> GetClientCacheInfoAsync(string clientCode)
        {
            var result = await _context.Clients
                .Where(c => c.ClientCode == clientCode)
                .Select(c => new
                {
                    Client = c,
                    LatestDeployment = _context.ClientDeployments
                        .Where(d => d.ClientId == c.Id && d.IsActive)
                        .OrderByDescending(d => d.WebApiVersionCode)
                        .FirstOrDefault()
                })
                .Select(x => new ClientCacheInfo
                {
                    Id = x.Client.Id,
                    ClientCode = x.Client.ClientCode,
                    ClientName = x.Client.ClientName,
                    SecurityCode = x.Client.SecurityCode,
                    ClientSecretKey = x.Client.ClientSecretKey,
                    Status = x.Client.Status,
                    WebApiVersionCode = x.LatestDeployment != null ? x.LatestDeployment.WebApiVersionCode : null,
                    ApiUrl = x.LatestDeployment != null ? x.LatestDeployment.ApiUrl : null,
                })
                .FirstOrDefaultAsync();

            return result;
        }
    }
}
