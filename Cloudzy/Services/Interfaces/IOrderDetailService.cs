using Cloudzy.Models.ViewModels.AdminOrderDetail;

namespace Cloudzy.Services.Interfaces
{
    public interface IOrderDetailService
    {
        Task<DetailViewModel> GetOrderDetailByIdAsync(int orderId);
    }
}
