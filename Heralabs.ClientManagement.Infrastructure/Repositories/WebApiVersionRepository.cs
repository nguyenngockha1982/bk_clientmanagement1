using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Entities;
using Heralabs.ClientManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Heralabs.ClientManagement.Infrastructure.Repositories
{
    public class WebApiVersionRepository : IWebApiVersionRepository
    {
        private readonly AppDbContext _context;
        public WebApiVersionRepository(AppDbContext context) => _context = context;

        public async Task<WebApiVersion?> GetByVersionCodeAsync(string version)
            => await _context.WebApiVersions.FirstOrDefaultAsync(c => c.Version == version);
    }
}
