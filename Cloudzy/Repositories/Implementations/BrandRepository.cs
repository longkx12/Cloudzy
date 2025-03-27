using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class BrandRepository : IBrandRepository
    {
        private readonly DbCloudzyContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandRepository(DbCloudzyContext context,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task AddAsync(Brand entity)
        {
            await _context.Brands.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(p => p.BrandId == id);

            if (brand != null)
            {
                // Xóa ảnh trong thư mục wwwroot/images
                if (!string.IsNullOrEmpty(brand.BrandImg))
                {
                    string filePath = Path.Combine(_webHostEnvironment.WebRootPath, brand.BrandImg.TrimStart('/'));
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }

                // Xóa nhãn hàng khỏi database
                _context.Brands.Remove(brand);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands.FindAsync(id);
        }

        public async Task UpdateAsync(Brand entity)
        {
            _context.Brands.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
