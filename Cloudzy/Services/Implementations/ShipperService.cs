using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;

namespace Cloudzy.Services.Implementations
{
    public class ShipperService : IShipperService
    {
        private readonly IShipperRepository _shipperRepository;

        public ShipperService(IShipperRepository shipperRepository)
        {
            _shipperRepository = shipperRepository;
        }

        public async Task<IEnumerable<User>> GetAllShippersAsync()
        {
            return await _shipperRepository.GetAllShippersAsync();
        }

        public async Task<IEnumerable<Order>> GetProcessingOrdersAsync()
        {
            return await _shipperRepository.GetProcessingOrdersAsync();
        }

        public async Task<IEnumerable<Order>> GetShippingOrdersByShipperIdAsync(int shipperId)
        {
            return await _shipperRepository.GetShippingOrdersByShipperIdAsync(shipperId);
        }

        public async Task<IEnumerable<Order>> GetDeliveredOrdersByShipperIdAsync(int shipperId)
        {
            return await _shipperRepository.GetDeliveredOrdersByShipperIdAsync(shipperId);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _shipperRepository.GetOrderByIdAsync(orderId);
        }

        public async Task<bool> AcceptOrderAsync(int orderId, int shipperId)
        {
            return await _shipperRepository.AcceptOrderAsync(orderId, shipperId);
        }

        public async Task<bool> DeliverOrderAsync(int orderId, int shipperId)
        {
            return await _shipperRepository.DeliverOrderAsync(orderId, shipperId);
        }
    }
}