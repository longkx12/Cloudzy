using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cloudzy.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartApiController : ControllerBase
    {
        private readonly DbCloudzyContext _context;

        public CartApiController(DbCloudzyContext context)
        {
            _context = context;
        }
        [HttpPost("AddToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                var userIdClaim = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return BadRequest(new { success = false, message = "Không thể xác định người dùng. Vui lòng đăng nhập lại." });
                }
                var userId = int.Parse(userIdClaim);

                // Kiểm tra số lượng tồn kho
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.VariantId == request.VariantId);

                if (variant == null)
                {
                    return BadRequest(new { success = false, message = "Không tìm thấy sản phẩm." });
                }

                // Kiểm tra nếu số lượng yêu cầu vượt quá số lượng tồn kho
                if (request.Quantity > variant.Stock)
                {
                    return BadRequest(new { success = false, message = $"Số lượng không được vượt quá số lượng có sẵn ({variant.Stock})." });
                }

                // Kiểm tra xem người dùng đã có giỏ hàng chưa
                var cart = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                // Nếu chưa có giỏ hàng, tạo mới
                if (cart == null)
                {
                    cart = new ShoppingCart
                    {
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };

                    await _context.ShoppingCarts.AddAsync(cart);
                    await _context.SaveChangesAsync();
                }

                // Kiểm tra sản phẩm đã tồn tại trong giỏ hàng chưa
                var existingCartItem = await _context.CartItems
                    .FirstOrDefaultAsync(ci => ci.CartId == cart.CartId && ci.VariantId == request.VariantId);

                if (existingCartItem != null)
                {
                    // Kiểm tra xem tổng số lượng sau khi thêm có vượt quá tồn kho không
                    if (existingCartItem.Quantity + request.Quantity > variant.Stock)
                    {
                        return BadRequest(new { success = false, message = $"Tổng số lượng trong giỏ hàng không được vượt quá số lượng có sẵn ({variant.Stock})." });
                    }

                    // Nếu đã tồn tại, tăng số lượng
                    existingCartItem.Quantity += request.Quantity;
                    _context.CartItems.Update(existingCartItem);
                }
                else
                {
                    // Nếu chưa tồn tại, thêm mới
                    var cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        VariantId = request.VariantId,
                        Quantity = request.Quantity
                    };
                    await _context.CartItems.AddAsync(cartItem);
                }

                await _context.SaveChangesAsync();

                // Đếm tổng số sản phẩm trong giỏ hàng để trả về
                var totalItems = await _context.CartItems
                    .Where(ci => ci.CartId == cart.CartId)
                    .SumAsync(ci => ci.Quantity);

                return Ok(new { success = true, message = "Thêm vào giỏ hàng thành công!", totalItems });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi thêm vào giỏ hàng: " + ex.Message });
            }
        }

        [HttpGet("GetCartCount")]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Ok(new { success = true, totalItems = 0 });
                }

                var userIdClaim = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Ok(new { success = true, totalItems = 0 });
                }
                var userId = int.Parse(userIdClaim);

                // Tìm giỏ hàng của người dùng
                var cart = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return Ok(new { success = true, totalItems = 0 });
                }

                // Đếm tổng số sản phẩm trong giỏ hàng
                var totalItems = await _context.CartItems
                    .Where(ci => ci.CartId == cart.CartId)
                    .SumAsync(ci => ci.Quantity);

                return Ok(new { success = true, totalItems });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = "Lỗi khi lấy số lượng giỏ hàng: " + ex.Message });
            }
        }
    }

    public class AddToCartRequest
    {
        public int VariantId { get; set; }
        public int Quantity { get; set; }
    }
}

