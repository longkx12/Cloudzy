using Cloudzy.Models.ViewModels.AdminOrder;
using X.PagedList;

namespace Cloudzy.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize);
        Task<IPagedList<ListViewModel>> GetReturnRequestsAsync(int pageNumber, int pageSize);
        Task<bool> UpdateOrderStatusAndShipperAsync(int orderId, string status, int? shipperId);
    }
}