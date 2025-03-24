using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminBrand;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IBrandRepository _brandRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public BrandService(IBrandRepository brandRepository, IWebHostEnvironment webHostEnvironment)
        {
            _brandRepository = brandRepository;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task AddAsync(BrandCreateViewModel model)
        {
            string? imgPath = null;

            if (model.BrandImg == null || model.BrandImg.Length == 0)
            {
                throw new ArgumentException("Bạn cần phải tải lên một file ảnh.");
            }

            // Lấy đường dẫn thư mục wwwroot/images
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Đảm bảo thư mục tồn tại
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Tạo tên file duy nhất theo timestamp
            string fileName = Path.GetFileNameWithoutExtension(model.BrandImg.FileName);
            string extension = Path.GetExtension(model.BrandImg.FileName);
            string uniqueFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            // Lưu file ảnh
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.BrandImg.CopyToAsync(fileStream);
            }

            // Lưu đường dẫn ảnh vào database (chỉ chứa đường dẫn tương đối)
            imgPath = "/images/" + uniqueFileName;

            var brand = new Brand
            {
                BrandName = model.BrandName,
                BrandImg = imgPath,
                Description = model.Description
            };

            await _brandRepository.AddAsync(brand);
        }

        public async Task DeleteAsync(int id)
        {
            await _brandRepository.DeleteAsync(id);
        }

        public async Task<IPagedList<BrandListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var brands = await _brandRepository.GetAllAsync();
            var pageBrands = brands.Select((b, index) => new BrandListViewModel
            {
                BrandId = b.BrandId,
                STT = index + 1,
                BrandName = b.BrandName,
                BrandImg = b.BrandImg,
                Description = b.Description
            }).ToPagedList(pageNumber, pageSize);
            return pageBrands;
        }

        public async Task<BrandEditViewModel> GetByIdAsync(int id)
        {
            var brand = await _brandRepository.GetByIdAsync(id);
            if (brand == null) return null;

            return new BrandEditViewModel
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                ExistingImg = brand.BrandImg,
                Description = brand.Description
            };
        }

        public async Task UpdateAsync(BrandEditViewModel model)
        {
            var brand = await _brandRepository.GetByIdAsync(model.BrandId);
            if (brand == null) return;

            string? imgPath = brand.BrandImg; // Giữ nguyên ảnh cũ nếu không có ảnh mới

            if (model.BrandImg != null && model.BrandImg.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

                // Đảm bảo thư mục tồn tại
                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                // Xóa ảnh cũ (nếu có)
                if (!string.IsNullOrEmpty(brand.BrandImg))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, brand.BrandImg.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                // Tạo tên file ảnh mới theo timestamp
                string fileName = Path.GetFileNameWithoutExtension(model.BrandImg.FileName);
                string extension = Path.GetExtension(model.BrandImg.FileName);
                string uniqueFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                // Lưu ảnh mới
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.BrandImg.CopyToAsync(fileStream);
                }

                imgPath = "/images/" + uniqueFileName;
            }

            brand.BrandName = model.BrandName;
            brand.BrandImg = imgPath;
            brand.Description = model.Description;

            await _brandRepository.UpdateAsync(brand);
        }
    }
}
