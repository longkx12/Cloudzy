using Cloudzy.Repositories.Interfaces;
using Cloudzy.Models.ViewModels.AdminOrderDetail;
using Cloudzy.Services.Interfaces;

namespace Cloudzy.Services.Implementations
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;

        public OrderDetailService(IOrderRepository orderRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<DetailViewModel> GetOrderDetailByIdAsync(int orderId)
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
            {
                return null;
            }

            var orderDetails = await _orderDetailRepository.GetByOrderIdAsync(orderId);
            if (orderDetails == null || !orderDetails.Any())
            {
                return null;
            }

            var orderItems = orderDetails.Select(od => new OrderItemViewModel
            {
                OrderDetailId = od.OrderDetailId,
                ProductName = od.Variant?.Product?.ProductName,
                ProductImage = od.Variant?.Product?.ProductImages.FirstOrDefault()?.ImageUrl,
                SizeName = od.Variant?.Size?.SizeName,
                Quantity = od.Quantity,
                Price = od.Price
            }).ToList();

            decimal subtotal = orderItems.Sum(item => item.TotalPrice);
            decimal discountAmount = 0;

            if (order.DiscountCode?.VoucherType != null)
            {
                var voucherType = order.DiscountCode.VoucherType;
                if (voucherType.MaximumValue.HasValue)
                {
                    discountAmount = Math.Min(subtotal * voucherType.Value / 100, voucherType.MaximumValue.Value);
                }
                else
                {
                    discountAmount = subtotal * voucherType.Value / 100;
                }
            }

            var viewModel = new DetailViewModel
            {
                OrderId = order.OrderId,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                PaymentMethod = order.PaymentMethod,
                CustomerName = order.User?.Fullname,
                PhoneNumber = order.User?.PhoneNumber,
                Email = order.User?.Email,
                ShippingAddress = order.Address,
                VoucherCode = order.DiscountCode?.Code,
                DiscountAmount = discountAmount,
                Subtotal = subtotal,
                TotalPrice = order.TotalPrice,
                OrderItems = orderItems
            };

            return viewModel;
        }
    }
}
