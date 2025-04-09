using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.Cart;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _repository;
        private readonly DbCloudzyContext _context;
        public CartItemService(ICartItemRepository repository, DbCloudzyContext context)
        {
            _context = context;
            _repository = repository;
        }

        public async Task AddAsync(CartItemCreateViewModel model)
        {
            if (!await _context.ShoppingCarts.AnyAsync(i => i.CartId == model.CartId))
            {
                throw new Exception("Không tìm thấy giỏ hàng");
            }
            var cart = new CartItem
            {
                CartId = model.CartId,
                VariantId = model.VariantId,
                Quantity = model.Quantity
            };
            await _repository.AddAsync(cart);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        public async Task<IPagedList<CartItemListViewModel>> GetAllAsync(int cartId, int pageNumber, int pageSize)
        {
            var cart = await _repository.GetAllAsync(cartId);
            return cart.Select((c, index) => new CartItemListViewModel
            {
                STT = index + 1,
                CartItemId = c.CartItemId,
                CartId = c.CartId,
                ProductName = c.Variant?.Product?.ProductName ?? "Unknown",
                SizeName = c.Variant?.Size?.SizeName ?? "Unknown",
                Quantity = c.Quantity,
                TotalPrice = (c.Variant?.Product?.Price ?? 0) * c.Quantity
            }).ToPagedList(pageNumber, pageSize);
        }

        public async Task<CartItemEditViewModel> GetByIdAsync(int id)
        {
            var cart = await _repository.GetByIdAsync(id);
            if (cart == null) return null;

            return new CartItemEditViewModel
            {
                CartId = cart.CartId,
                CartItemId = cart.CartItemId,
                VariantId = cart.VariantId,
                Quantity = cart.Quantity
            };
        }

        public async Task UpdateAsync(CartItemEditViewModel model)
        {
            var cart = await _repository.GetByIdAsync(model.CartItemId);
            if (cart != null)
            {
                cart.CartId = model.CartId;
                cart.CartItemId = model.CartItemId;
                cart.VariantId = model.VariantId;
                cart.Quantity = model.Quantity;

                await _repository.UpdateAsync(cart);
            }
        }
    }
}
