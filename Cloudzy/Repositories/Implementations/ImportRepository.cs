using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ImportRepository : Repository<Import>, IImportRepository
    {
        public ImportRepository(DbCloudzyContext dbCloudzyContext) : base(dbCloudzyContext)
        {
        }

        public override async Task<IEnumerable<Import>> GetAllAsync()
        {
            return await _dbSet.Include(i => i.Supplier).ToListAsync();
        }
    }
}
