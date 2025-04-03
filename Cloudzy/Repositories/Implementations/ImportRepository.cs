using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class ImportRepository : IImportRepository
    {
        private readonly DbCloudzyContext _context;
        public ImportRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task AddAsync(Import entity)
        {
            _context.Imports.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var import = await _context.Imports.FindAsync(id);
            if (import != null)
            {
                _context.Imports.Remove(import);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Import>> GetAllAsync()
        {
            return await _context.Imports.Include(i => i.Supplier).ToListAsync();
        }

        public async Task<Import> GetByIdAsync(int id)
        {
            return await _context.Imports.Include(i => i.Supplier)
                                         .FirstOrDefaultAsync(i => i.ImportId == id);
        }

        public async Task UpdateAsync(Import entity)
        {
            _context.Imports.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
