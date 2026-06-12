using Heralabs.ClientManagement.Core.Interfaces;
using Heralabs.ClientManagement.Domain.Entities;
using Heralabs.ClientManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Heralabs.ClientManagement.Infrastructure.Repositories
{
    public class MobileAppVersionRepository : IMobileAppVersionRepository
    {
        private readonly AppDbContext _context;
        public MobileAppVersionRepository(AppDbContext context) => _context = context;

        public async Task<IList<MobileAppVersion>> GetAllActivepVersionsAsync()
            => await _context.MobileAppVersions.Where(m => m.IsActive).ToListAsync();
    }
}
