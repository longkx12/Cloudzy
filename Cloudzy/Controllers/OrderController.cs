using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.Order;
using Cloudzy.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cloudzy.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DbCloudzyContext _context;
        private readonly IConfiguration _configuration;

        public OrderController(DbCloudzyContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<IActionResult> Checkout()
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = await _context.ShoppingCarts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !await _context.CartItems.AnyAsync(ci => ci.CartId == cart.CartId))
            {
                return RedirectToAction("Details", "Cart");
            }

            var cartItems = await _context.CartItems
                .Include(ci => ci.Variant)
                    .ThenInclude(v => v.Product)
                        .ThenInclude(p => p.ProductImages)
                .Include(ci => ci.Variant)
                    .ThenInclude(v => v.Size)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();

            var viewModel = new CheckoutViewModel
            {
                UserId = userId,
                FullName = user.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber ?? "",
                Address = user.Address ?? "",
                OrderItems = cartItems.Select(ci => new OrderItemViewModel
                {
                    ProductId = ci.Variant.ProductId.Value,
                    ProductName = ci.Variant.Product.ProductName,
                    SizeName = ci.Variant.Size.SizeName,
                    ProductImage = ci.Variant.Product.ProductImages.FirstOrDefault()?.ImageUrl ?? "",
                    Price = ci.Variant.Product.DiscountPrice ?? 0,
                    Quantity = ci.Quantity,
                    TotalPrice = (ci.Variant.Product.DiscountPrice ?? 0) * ci.Quantity,
                    VariantId = ci.VariantId ?? 0
                }).ToList()
            };

            // Tính tổng tiền
            viewModel.SubTotal = viewModel.OrderItems.Sum(item => item.TotalPrice);

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

            viewModel.Total = viewModel.SubTotal - viewModel.VoucherDiscount;

            viewModel.Addresses = new List<UserAddressViewModel>
            {
                new UserAddressViewModel
                {
                    Id = 1,
                    FullName = user.Fullname,
                    PhoneNumber = user.PhoneNumber ?? "",
                    Address = user.Address ?? "",
                    IsDefault = true
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> PlaceOrder(OrderRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Checkout");
            }

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
                return RedirectToAction("Details", "Cart");
            }

            var cartItems = await _context.CartItems
                .Include(ci => ci.Variant)
                    .ThenInclude(v => v.Product)
                .Include(ci => ci.Variant)
                    .ThenInclude(v => v.Size)
                .Where(ci => ci.CartId == cart.CartId)
                .ToListAsync();

            if (!cartItems.Any())
            {
                return RedirectToAction("Details", "Cart");
            }

            decimal subtotal = cartItems.Sum(ci => (ci.Variant.Product.DiscountPrice ?? 0) * ci.Quantity);

            // Lấy thông tin voucher nếu có
            int? discountCodeId = null;
            decimal voucherDiscount = 0;

            if (!string.IsNullOrEmpty(model.VoucherCode) && model.VoucherTypeId > 0)
            {
                var discountCode = await _context.DiscountCodes
                    .Include(d => d.VoucherType)
                    .FirstOrDefaultAsync(d => d.Code == model.VoucherCode);

                if (discountCode != null)
                {
                    discountCodeId = discountCode.DiscountCodeId;
                    var voucherType = discountCode.VoucherType;

                    if (subtotal >= voucherType.MinimumValue)
                    {
                        if (voucherType.Value < 1)
                        {
                            voucherDiscount = subtotal * voucherType.Value;

                            if (voucherType.MaximumValue.HasValue && voucherDiscount > voucherType.MaximumValue.Value)
                            {
                                voucherDiscount = voucherType.MaximumValue.Value;
                            }
                        }
                        else
                        {
                            voucherDiscount = voucherType.Value;

                            if (voucherDiscount > subtotal)
                            {
                                voucherDiscount = subtotal;
                            }
                        }
                    }
                }
            }

            decimal totalPrice = subtotal - voucherDiscount;

            string orderStatus = model.PaymentMethod == "COD" ? "Pending" : "Pending Payment";

            var order = new Order
            {
                UserId = userId,
                Status = orderStatus,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                PaymentMethod = model.PaymentMethod,
                Address = model.Address,
                TotalPrice = totalPrice,
                DiscountCodeId = discountCodeId
            };

            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            foreach (var cartItem in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    VariantId = cartItem.VariantId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Variant.Product.DiscountPrice ?? 0
                };

                await _context.OrderDetails.AddAsync(orderDetail);

                var variant = await _context.ProductVariants.FindAsync(cartItem.VariantId);
                if (variant != null)
                {
                    variant.Stock -= cartItem.Quantity;
                    _context.ProductVariants.Update(variant);
                }
            }

            await _context.SaveChangesAsync();

            if (model.PaymentMethod == "COD")
            {
                _context.CartItems.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                HttpContext.Session.Remove("VoucherCode");
                HttpContext.Session.Remove("VoucherType");
                HttpContext.Session.Remove("VoucherDiscount");

                TempData["ToastMessage"] = "Đặt hàng thành công! Đơn hàng của bạn đang được xử lý.";
                TempData["ToastType"] = "success";
                return RedirectToAction("Index", "Home");
            }
            else
            {
                HttpContext.Session.SetInt32("PendingOrderId", order.OrderId);

                var vnpayUrl = CreateVnPayUrl(order.OrderId, totalPrice);
                return Redirect(vnpayUrl);
            }
        }

        [AllowAnonymous]
        public async Task<IActionResult> VnPayReturn([FromQuery] VnPayResponseViewModel response)
        {
            if (response == null)
            {
                TempData["ToastMessage"] = "Không nhận được dữ liệu trả về từ cổng thanh toán.";
                TempData["ToastType"] = "error";
                return RedirectToAction("Index", "Home");
            }

            try
            {
                VnPayLibrary vnpay = new VnPayLibrary();
                foreach (var prop in typeof(VnPayResponseViewModel).GetProperties())
                {
                    string name = prop.Name;
                    if (name.StartsWith("vnp_"))
                    {
                        var value = prop.GetValue(response, null)?.ToString();
                        if (!string.IsNullOrEmpty(value))
                        {
                            vnpay.AddResponseData(name, value);
                        }
                    }
                }

                string vnp_HashSecret = _configuration["VnPay:HashSecret"];
                bool checkSignature = vnpay.ValidateSignature(response.vnp_SecureHash, vnp_HashSecret);

                if (checkSignature)
                {
                    int orderId = int.Parse(response.vnp_TxnRef);
                    var order = await _context.Orders.FindAsync(orderId);

                    if (order != null)
                    {
                        if (response.vnp_ResponseCode == "00" && response.vnp_TransactionStatus == "00")
                        {
                            order.Status = "Paid";
                            order.UpdatedAt = DateTime.Now;

                            int userId = order.UserId ?? 0;

                            var cart = await _context.ShoppingCarts
                                .FirstOrDefaultAsync(c => c.UserId == userId);

                            if (cart != null)
                            {
                                var cartItems = await _context.CartItems
                                    .Where(ci => ci.CartId == cart.CartId)
                                    .ToListAsync();

                                if (cartItems.Any())
                                {
                                    _context.CartItems.RemoveRange(cartItems);
                                }
                            }

                            HttpContext.Session.Remove("VoucherCode");
                            HttpContext.Session.Remove("VoucherType");
                            HttpContext.Session.Remove("VoucherDiscount");
                            HttpContext.Session.Remove("PendingOrderId");

                            TempData["ToastMessage"] = "Thanh toán thành công! Đơn hàng của bạn đang được xử lý.";
                            TempData["ToastType"] = "success";
                        }
                        else
                        {
                            order.Status = "Cancelled";
                            order.UpdatedAt = DateTime.Now;

                            var orderDetails = await _context.OrderDetails
                                .Where(od => od.OrderId == orderId)
                                .ToListAsync();

                            foreach (var detail in orderDetails)
                            {
                                var variant = await _context.ProductVariants.FindAsync(detail.VariantId);
                                if (variant != null)
                                {
                                    variant.Stock += detail.Quantity;
                                    _context.ProductVariants.Update(variant);
                                }
                            }

                            HttpContext.Session.Remove("PendingOrderId");
                            TempData["ToastMessage"] = "Thanh toán thất bại hoặc đã bị hủy.";
                            TempData["ToastType"] = "error";
                        }

                        _context.Orders.Update(order);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        TempData["ToastMessage"] = "Không tìm thấy đơn hàng.";
                        TempData["ToastType"] = "error";
                    }
                }
                else
                {
                    TempData["ToastMessage"] = "Chữ ký không hợp lệ.";
                    TempData["ToastType"] = "error";
                }
            }
            catch (Exception ex)
            {
                TempData["ToastMessage"] = "Có lỗi xảy ra khi xử lý kết quả thanh toán: " + ex.Message;
                TempData["ToastType"] = "error";
            }

            return RedirectToAction("Index", "Home");
        }

        private string CreateVnPayUrl(int orderId, decimal amount)
        {
            string vnp_Returnurl = _configuration["VnPay:ReturnUrl"];
            string vnp_Url = _configuration["VnPay:PaymentUrl"];
            string vnp_TmnCode = _configuration["VnPay:TmnCode"];
            string vnp_HashSecret = _configuration["VnPay:HashSecret"];

            if (string.IsNullOrEmpty(vnp_Returnurl) ||
                string.IsNullOrEmpty(vnp_Url) ||
                string.IsNullOrEmpty(vnp_TmnCode) ||
                string.IsNullOrEmpty(vnp_HashSecret))
            {
                throw new InvalidOperationException("Cấu hình VNPay không đầy đủ");
            }

            VnPayLibrary vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", Convert.ToInt64(amount * 100).ToString());
            vnpay.AddRequestData("vnp_BankCode", "");
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1");
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang: " + orderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", orderId.ToString());

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

            return paymentUrl;
        }
    }
}