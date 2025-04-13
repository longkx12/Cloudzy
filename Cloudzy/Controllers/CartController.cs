using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.CartDetails;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cloudzy.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly DbCloudzyContext _context;
        private readonly ICartItemService _service;
        public CartController(DbCloudzyContext context, ICartItemService service)
        {
            _context = context;
            _service = service;
        }

        public async Task<IActionResult> Details()
        {
            // Lấy ID của người dùng đã đăng nhập
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            var cart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId);

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

            var cartItems = await _context.CartItems
                .Include(ci => ci.Variant)
                    .ThenInclude(v => v.Product)
                .Include(ci => ci.Variant)
                    .ThenInclude(v => v.Size)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();

            var viewModel = new CartDetailsViewModel
            {
                CartId = cart.CartId,
                CartItems = cartItems.Select(ci => new CartItemDisplayViewModel
                {
                    CartItemId = ci.CartItemId,
                    ProductName = ci.Variant.Product.ProductName,
                    ProductImage = ci.Variant.Product.ProductImages.Select(img => img.ImageUrl).FirstOrDefault() ?? "",
                    SizeName = ci.Variant.Size.SizeName,
                    Price = ci.Variant.Product.Price,
                    Quantity = ci.Quantity,
                    TotalPrice = ci.Variant.Product.Price * ci.Quantity,
                    VariantId = ci.VariantId,
                    Stock = ci.Variant.Stock
                }).ToList()
            };

            viewModel.SubTotal = viewModel.CartItems.Sum(item => item.TotalPrice) ?? 0;
            viewModel.ShippingCost = 20000; // Mặc định phí vận chuyển

            // Kiểm tra voucher trong session
            if (HttpContext.Session.GetString("VoucherCode") is string voucherCode && !string.IsNullOrEmpty(voucherCode))
            {
                viewModel.VoucherCode = voucherCode;
                viewModel.VoucherTypeId = HttpContext.Session.GetInt32("VoucherType") ?? 0;

                if (decimal.TryParse(HttpContext.Session.GetString("VoucherDiscount"), out decimal discount))
                {
                    viewModel.VoucherDiscount = discount;
                }
            }

            // Lấy ID phương thức vận chuyển nếu có
            viewModel.ShippingMethodId = HttpContext.Session.GetInt32("ShippingMethodId") ?? 2;

            // Tính tổng dựa trên tổng phụ, phí vận chuyển và bất kỳ giảm giá nào
            viewModel.Total = viewModel.SubTotal + viewModel.ShippingCost - viewModel.VoucherDiscount;

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var cartItem = await _context.CartItems
                    .Include(ci => ci.Variant)
                    .FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);

                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
                }

                if (quantity > cartItem.Variant.Stock)
                {
                    return Json(new { success = false, message = $"Số lượng không được vượt quá số lượng có sẵn ({cartItem.Variant.Stock})." });
                }

                if (quantity <= 0)
                {
                    _context.CartItems.Remove(cartItem);
                }
                else
                {
                    cartItem.Quantity = quantity;
                    _context.CartItems.Update(cartItem);
                }

                await _context.SaveChangesAsync();

                // Lấy tổng giỏ hàng đã cập nhật
                var cartId = cartItem.CartId;
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Variant)
                        .ThenInclude(v => v.Product)
                    .Where(ci => ci.CartId == cartId)
                    .ToListAsync();

                var subTotal = cartItems.Sum(ci => ci.Variant.Product.Price * ci.Quantity);
                var shippingCost = 20000;
                var total = subTotal + shippingCost;

                return Json(new
                {
                    success = true,
                    message = "Cập nhật số lượng thành công!",
                    itemTotal = cartItem.Variant.Product.Price * quantity,
                    subTotal = subTotal,
                    total = total
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi cập nhật số lượng: " + ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int cartItemId)
        {
            try
            {
                var cartItem = await _context.CartItems.FindAsync(cartItemId);
                if (cartItem == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng." });
                }

                var cartId = cartItem.CartId;
                _context.CartItems.Remove(cartItem);
                await _context.SaveChangesAsync();

                // Lấy tổng giỏ hàng đã cập nhật
                var cartItems = await _context.CartItems
                    .Include(ci => ci.Variant)
                        .ThenInclude(v => v.Product)
                    .Where(ci => ci.CartId == cartId)
                    .ToListAsync();

                var subTotal = cartItems.Sum(ci => ci.Variant.Product.Price * ci.Quantity);
                var shippingCost = cartItems.Any() ? 20000 : 0;
                var total = subTotal + shippingCost;

                // Lấy tổng số mục để cập nhật giỏ hàng
                var totalItems = cartItems.Sum(ci => ci.Quantity);

                return Json(new
                {
                    success = true,
                    message = "Đã xóa sản phẩm khỏi giỏ hàng!",
                    subTotal = subTotal,
                    total = total,
                    itemCount = totalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa sản phẩm: " + ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableVouchers()
        {
            try
            {
                // Lấy ID của người dùng đã đăng nhập
                var userIdClaim = User.FindFirstValue("UserId");
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Json(new { success = false, message = "User not authenticated" });
                }

                var userId = int.Parse(userIdClaim);

                // Lấy tổng phụ của giỏ hàng để kiểm tra giá trị đơn hàng tối thiểu cho voucher
                var cart = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cart == null)
                {
                    return Json(new { success = true, vouchers = new List<object>() });
                }

                var cartSubtotal = await _context.CartItems
                    .Include(ci => ci.Variant)
                        .ThenInclude(v => v.Product)
                    .Where(ci => ci.CartId == cart.CartId)
                    .SumAsync(ci => ci.Variant.Product.Price * ci.Quantity);

                // Lấy các mã giảm giá khả dụng (voucher)
                var availableVouchers = await _context.DiscountCodes
                    .Include(d => d.VoucherType)
                    .Where(d =>
                        d.StartDate <= DateTime.Now &&
                        d.EndDate >= DateTime.Now &&
                        (d.Quantity == 0 ||
                         d.Quantity > _context.Orders.Count(o => o.DiscountCodeId == d.DiscountCodeId)))
                    .Select(d => new
                    {
                        code = d.Code,
                        voucherTypeId = d.VoucherTypeId,
                        value = d.VoucherType.Value,
                        minimumValue = d.VoucherType.MinimumValue,
                        maximumValue = d.VoucherType.MaximumValue
                    })
                    .ToListAsync();

                return Json(new { success = true, vouchers = availableVouchers });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CheckVoucher(string code, decimal subtotal)
        {
            try
            {
                var voucher = await _context.DiscountCodes
                    .Include(d => d.VoucherType)
                    .FirstOrDefaultAsync(d => d.Code == code);

                if (voucher == null)
                {
                    return Json(new { success = false, message = "Mã voucher không tồn tại" });
                }

                if (voucher.StartDate > DateTime.Now || voucher.EndDate < DateTime.Now)
                {
                    return Json(new { success = false, message = "Mã voucher đã hết hạn hoặc chưa có hiệu lực" });
                }

                if (voucher.Quantity > 0)
                {
                    var usedCount = await _context.Orders.CountAsync(o => o.DiscountCodeId == voucher.DiscountCodeId);
                    if (usedCount >= voucher.Quantity)
                    {
                        return Json(new { success = false, message = "Mã voucher đã hết lượt sử dụng" });
                    }
                }

                if (subtotal < voucher.VoucherType.MinimumValue)
                {
                    return Json(new
                    {
                        success = false,
                        message = $"Mã voucher này chỉ áp dụng cho đơn hàng từ {voucher.VoucherType.MinimumValue:N0}đ"
                    });
                }

                decimal discountAmount = 0;
                int voucherType;

                // Xác định loại voucher dựa vào value
                decimal value = voucher.VoucherType.Value;

                if (value > 0 && value < 1)
                {
                    // Phần trăm
                    voucherType = 1;
                    discountAmount = subtotal * value;

                    // Áp dụng giới hạn tối đa nếu có
                    if (voucher.VoucherType.MaximumValue.HasValue && discountAmount > voucher.VoucherType.MaximumValue.Value)
                    {
                        discountAmount = voucher.VoucherType.MaximumValue.Value;
                    }
                }
                else if (value >= 1)
                {
                    // Số tiền cố định
                    voucherType = 3;
                    discountAmount = value;

                    // Đảm bảo không vượt quá giá trị đơn hàng
                    if (discountAmount > subtotal)
                    {
                        discountAmount = subtotal;
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Mã voucher không hợp lệ" });
                }

                return Json(new
                {
                    success = true,
                    voucherType = voucherType,
                    discountAmount = discountAmount
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public IActionResult ApplyVoucher(string code, int voucherType, decimal discountAmount)
        {
            try
            {
                // Lưu thông tin voucher trong session
                HttpContext.Session.SetString("VoucherCode", code);
                HttpContext.Session.SetInt32("VoucherType", voucherType);
                HttpContext.Session.SetString("VoucherDiscount", discountAmount.ToString());

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
