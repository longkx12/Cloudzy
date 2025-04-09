using Cloudzy.Models.Domain;

namespace Cloudzy.Repositories.Interfaces
{
    public interface IImportDetailRepository : IRepository<ImportDetail>
    {
        Task<IEnumerable<ImportDetail>> GetAllAsync(int importId);
    }
}
