using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.Cart;
using Cloudzy.Repositories.Implementations;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class ShoppingCartService : IShoppingCartService
    {
        private readonly IShoppingCartRepository _repository;
        public ShoppingCartService(IShoppingCartRepository repository)
        {
            _repository = repository;
        }

        public async Task AddAsync(ShoppingCartCreateViewModel model)
        {
            //Kiểm tra trùng giỏ hàng
            var existingCart = (await _repository.GetAllAsync())
                .FirstOrDefault(c => c.UserId == model.UserId);
            if (existingCart != null)
            {
                throw new Exception("Giỏ hàng đã tồn tại");
            }

            var shoppingCart = new ShoppingCart
            {
                UserId = model.UserId,
                CreatedAt = model.CreatedAt
            };
            await _repository.AddAsync(shoppingCart);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IPagedList<ShoppingCartListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var carts = await _repository.GetAllAsync();
            return carts.Select((c, index) => new ShoppingCartListViewModel
            {
                STT = index + 1,
                CartId = c.CartId,
                Fullname = c.User.Fullname,
                CreatedAt = c.CreatedAt
            }).ToPagedList(pageNumber, pageSize);
        }

        public Task<ShoppingCart> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    }
}
