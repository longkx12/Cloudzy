using Cloudzy.Models.ViewModels.UserProfile;
namespace Cloudzy.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<EditViewModel> GetByIdAsync(int id);
        Task UpdateAsync(EditViewModel model);
        Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    }
}
