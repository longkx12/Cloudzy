using Cloudzy.Models.ViewModels.AdminOrder;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;

        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }
        public async Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var orders = await _repository.GetAllAsync();
            var pageOrder = orders.Select((o, index) => new ListViewModel
            {
                STT = index + 1,
                OrderId = o.OrderId,
                PhoneNumber = o.User?.PhoneNumber,
                Email = o.User?.Email,
                CreatedAt = o.CreatedAt,
                VoucherCode = o.DiscountCode?.Code,
                PaymentMethod = o.PaymentMethod,
                Status = o.Status
            }).ToPagedList(pageNumber, pageSize);
            return pageOrder;
        }
    }
}
