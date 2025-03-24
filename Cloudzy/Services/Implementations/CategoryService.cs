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
        public async Task AddAsync(CategoryViewModel model)
        {
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

        public async Task<IPagedList<CategoryListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var category = await _categoryRepository.GetAllAsync();
            var pageCategories = category.Select((c, index) => new CategoryListViewModel
            {
                CategoryId = c.CategoryId,
                STT = index + 1,
                CategoryName = c.CategoryName,
                Description = c.Description
            }).ToPagedList(pageNumber, pageSize);
            return pageCategories;
        }

        public async Task<CategoryEditViewModel> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryEditViewModel
            {
                CategoryName = category.CategoryName,
                Description = category.Description
            };
        }

        public async Task UpdateAsync(CategoryEditViewModel model)
        {
            var category = await _categoryRepository.GetByIdAsync(model.CategoryId);
            if(category != null)
            {
                category.CategoryName = model.CategoryName;
                category.Description = model.Description;
                await _categoryRepository.UpdateAsync(category);
            }
        }
    }
}
