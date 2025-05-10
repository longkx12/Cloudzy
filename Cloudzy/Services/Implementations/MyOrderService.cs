using Cloudzy.Models.ViewModels.MyOrder;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;

namespace Cloudzy.Services.Implementations
{
    public class MyOrderService : IMyOrderService
    {
        private readonly IMyOrderRepository _repository;
        private readonly IProductRepository _productRepository;

        public MyOrderService(IMyOrderRepository repository, IProductRepository productRepository)
        {
            _repository = repository;
            _productRepository = productRepository;
        }

        public async Task<ListViewModel> GetOrdersByUserIdAsync(int userId, string status = "all")
        {
            var orders = await _repository.GetOrdersByUserIdAsync(userId);

            if (status != "all")
            {
                orders = orders.Where(o => o.Status.ToLower() == status.ToLower()).ToList();
            }

            var viewModel = new ListViewModel
            {
                Orders = new List<ViewModel>(),
                CurrentStatus = status
            };

            foreach (var order in orders)
            {
                var orderViewModel = new ViewModel
                {
                    OrderId = order.OrderId,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt,
                    TotalPrice = order.TotalPrice,
                    PaymentMethod = order.PaymentMethod,
                    Address = order.Address,
                    DiscountCode = order.DiscountCode?.Code,
                    Items = new List<ItemViewModel>()
                };

                foreach (var detail in order.OrderDetails)
                {
                    var product = await _productRepository.GetProductByVariantIdAsync(detail.VariantId ?? 0);
                    var size = detail.Variant?.Size?.SizeName ?? "N/A";
                    var imageUrl = product?.ProductImages.FirstOrDefault()?.ImageUrl;

                    orderViewModel.Items.Add(new ItemViewModel
                    {
                        OrderDetailId = detail.OrderDetailId,
                        ProductId = product?.ProductId ?? 0,
                        ProductName = product?.ProductName ?? "Unknown Product",
                        ImageUrl = imageUrl,
                        Size = size,
                        Quantity = detail.Quantity,
                        Price = detail.Price
                    });
                }

                viewModel.Orders.Add(orderViewModel);
            }

            viewModel.Orders = viewModel.Orders.OrderByDescending(o => o.CreatedAt).ToList();

            return viewModel;
        }

        public async Task<DetailViewModel> GetOrderDetailByIdAsync(int orderId, int userId)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != userId)
            {
                return null;
            }

            var orderViewModel = new ViewModel
            {
                OrderId = order.OrderId,
                Status = order.Status,
                CreatedAt = order.CreatedAt,
                TotalPrice = order.TotalPrice,
                PaymentMethod = order.PaymentMethod,
                Address = order.Address,
                DiscountCode = order.DiscountCode?.Code,
                ReturnReason = order.ReturnReason,
                Items = new List<ItemViewModel>()
            };

            foreach (var detail in order.OrderDetails)
            {
                var product = await _productRepository.GetProductByVariantIdAsync(detail.VariantId ?? 0);
                var size = detail.Variant?.Size?.SizeName ?? "N/A";
                var imageUrl = product?.ProductImages.FirstOrDefault()?.ImageUrl;

                orderViewModel.Items.Add(new ItemViewModel
                {
                    OrderDetailId = detail.OrderDetailId,
                    ProductId = product?.ProductId ?? 0,
                    ProductName = product?.ProductName ?? "Unknown Product",
                    ImageUrl = imageUrl,
                    Size = size,
                    Quantity = detail.Quantity,
                    Price = detail.Price
                });
            }

            return new DetailViewModel
            {
                Order = orderViewModel
            };
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != userId || order.Status.ToLower() != "pending")
            {
                return false;
            }

            order.Status = "Cancelled";
            order.UpdatedAt = DateTime.Now;

            foreach (var detail in order.OrderDetails)
            {
                if (detail.Variant != null)
                {
                    detail.Variant.Stock += detail.Quantity;
                }
            }

            await _repository.UpdateOrderAsync(order);
            return true;
        }

        public async Task<bool> MarkOrderAsDeliveredAsync(int orderId, int userId)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != userId || order.Status.ToLower() != "shipping")
            {
                return false;
            }

            order.Status = "Delivered";
            order.UpdatedAt = DateTime.Now;

            await _repository.UpdateOrderAsync(order);
            return true;
        }

        public async Task<bool> ReturnOrderAsync(int orderId, int userId, string returnReason)
        {
            var order = await _repository.GetOrderByIdAsync(orderId);

            if (order == null || order.UserId != userId || order.Status.ToLower() != "delivered")
            {
                return false;
            }

            order.Status = "Returned";
            order.ReturnReason = returnReason;
            order.UpdatedAt = DateTime.Now;

            foreach (var detail in order.OrderDetails)
            {
                if (detail.Variant != null)
                {
                    detail.Variant.Stock += detail.Quantity;
                }
            }

            await _repository.UpdateOrderAsync(order);
            return true;
        }
    }
}