using Cloudzy.Models.ViewModels.MyOrder;

namespace Cloudzy.Services.Interfaces
{
    public interface IMyOrderService
    {
        Task<ListViewModel> GetOrdersByUserIdAsync(int userId, string status = "all");
        Task<DetailViewModel> GetOrderDetailByIdAsync(int orderId, int userId);
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<bool> MarkOrderAsDeliveredAsync(int orderId, int userId);
        Task<bool> ReturnOrderAsync(int orderId, int userId, string returnReason);
    }
}