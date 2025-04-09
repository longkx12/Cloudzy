using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminCategory;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task AddAsync(CreateViewModel model)
        {
            //Kiểm tra tên danh mục đã tồn tại
            var existingCategory = (await _categoryRepository.GetAllAsync())
                .FirstOrDefault(c => c.CategoryName.ToLower() == model.CategoryName.ToLower());
            if (existingCategory != null)
            {
                throw new Exception("Tên danh mục đã tồn tại!");
            }

            var category = new Category
            {
                CategoryName = model.CategoryName,
                Description = model.Description
            };
            await _categoryRepository.AddAsync(category);
        }

        public async Task DeleteAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var category = await _categoryRepository.GetAllAsync();
            var pageCategories = category.Select((c, index) => new ListViewModel
            {
                CategoryId = c.CategoryId,
                STT = index + 1,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToPagedList(pageNumber, pageSize);
            return pageCategories;
        }

        public async Task<IEnumerable<ListViewModel>> GetAllAsync()
        {
            var categorys = await _categoryRepository.GetAllAsync();
            return categorys.Select(c => new ListViewModel
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName
            }).ToList();
        }

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new EditViewModel
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
            if(category == null)
            {
                throw new Exception("Danh mục không tồn tại!");    
            }

            // Kiểm tra xem có danh mục khác trùng tên không
            var existingCategory = (await _categoryRepository.GetAllAsync())
                .FirstOrDefault(c => c.CategoryName.ToLower() == model.CategoryName.ToLower() && c.CategoryId != model.CategoryId);

            if (existingCategory != null)
            {
                throw new Exception("Tên danh mục đã tồn tại!");
            }

            category.CategoryName = model.CategoryName;
            category.Description = model.Description;
            await _categoryRepository.UpdateAsync(category);
        }
    }
}
