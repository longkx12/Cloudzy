using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ImportDetailRepository : Repository<ImportDetail>, IImportDetailRepository
    {
        public ImportDetailRepository(DbCloudzyContext dbCloudzyContext) : base(dbCloudzyContext)
        {
        }

        public async Task<IEnumerable<ImportDetail>> GetAllAsync(int importId)
        {
            return await _dbSet
                .Include(i => i.Product)
                    .ThenInclude(i=>i.ProductVariants)
                        .ThenInclude(i=>i.Size)
                .Where(i => i.ImportId == importId)
                .ToListAsync();
        }
    }
}
