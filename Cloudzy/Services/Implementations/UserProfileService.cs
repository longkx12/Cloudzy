using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.UserProfile;
using Cloudzy.Repositories.Implementations;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Cloudzy.Services.Implementations
{
    public class UserProfileService : IUserProfileService
    {
        private readonly IUserRepository _repository;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly IWebHostEnvironment _webHostEnvironment;
        public UserProfileService(IUserRepository repository, IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var user = await _repository.GetUserByIdAsync(id);
            if (user == null) return null;
            return new EditViewModel
            {
                UserId = user.UserId, 
                Fullname = user.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ExistingImg = user.UserImg,
                ProfileImage = null
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            var user = await _repository.GetUserByIdAsync(model.UserId);
            if (user == null) return;

            string? imgPath = user.UserImg;

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "profiles");

                if (!Directory.Exists(uploadFolder))
                {
                    Directory.CreateDirectory(uploadFolder);
                }

                if (!string.IsNullOrEmpty(user.UserImg))
                {
                    string oldFilePath = Path.Combine(_webHostEnvironment.WebRootPath, user.UserImg.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                string fileName = Path.GetFileNameWithoutExtension(model.ProfileImage.FileName);
                string extension = Path.GetExtension(model.ProfileImage.FileName);
                string uniqueFileName = fileName + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(fileStream);
                }

                imgPath = "/images/profiles/" + uniqueFileName;
            }

            user.Fullname = model.Fullname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.UserImg = imgPath;

            await _repository.UpdateUserAsync(user);
        }

        public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
        {
            var user = await _repository.GetUserByIdAsync(userId);
            if (user == null)
                throw new Exception("Không tìm thấy người dùng!");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.Password, currentPassword);
            if (verifyResult != PasswordVerificationResult.Success)
                return false;

            user.Password = _passwordHasher.HashPassword(user, newPassword);

            await _repository.UpdateUserAsync(user);
            return true;
        }
    }
}
