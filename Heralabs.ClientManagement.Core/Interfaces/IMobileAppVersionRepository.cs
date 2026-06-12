using Heralabs.ClientManagement.Domain.Entities;

namespace Heralabs.ClientManagement.Core.Interfaces
{
    public interface IMobileAppVersionRepository
    {
        Task<IList<MobileAppVersion>> GetAllActivepVersionsAsync();
    }
}
