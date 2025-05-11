using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.Address;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Cloudzy.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly DbCloudzyContext _context;

        public AddressController(DbCloudzyContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(AddAddressViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            if (model.IsDefault)
            {
                var defaultAddresses = await _context.UserAddresses
                    .Where(a => a.UserId == userId && a.IsDefault)
                    .ToListAsync();

                foreach (var address in defaultAddresses)
                {
                    address.IsDefault = false;
                    _context.UserAddresses.Update(address);
                }
            }
            else if (!await _context.UserAddresses.AnyAsync(a => a.UserId == userId))
            {
                model.IsDefault = true;
            }

            var userAddress = new UserAddress
            {
                UserId = userId,
                FullName = model.FullName,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                IsDefault = model.IsDefault,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _context.UserAddresses.AddAsync(userAddress);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new
                {
                    success = true,
                    addressId = userAddress.AddressId,
                    fullName = userAddress.FullName,
                    phoneNumber = userAddress.PhoneNumber,
                    address = userAddress.Address,
                    isDefault = userAddress.IsDefault
                });
            }

            return RedirectToAction("Checkout", "Order");
        }

        [HttpPost]
        public async Task<IActionResult> SetDefaultAddress(int addressId)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            var defaultAddresses = await _context.UserAddresses
                .Where(a => a.UserId == userId && a.IsDefault)
                .ToListAsync();

            foreach (var defaultAddress in defaultAddresses)
            {
                defaultAddress.IsDefault = false;
                _context.UserAddresses.Update(defaultAddress);
            }

            address.IsDefault = true;
            address.UpdatedAt = DateTime.Now;
            _context.UserAddresses.Update(address);

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Checkout", "Order");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            var userIdClaim = User.FindFirstValue("UserId");
            if (string.IsNullOrEmpty(userIdClaim))
            {
                return RedirectToAction("Login", "Account");
            }

            var userId = int.Parse(userIdClaim);

            var address = await _context.UserAddresses
                .FirstOrDefaultAsync(a => a.AddressId == addressId && a.UserId == userId);

            if (address == null)
            {
                return NotFound();
            }

            _context.UserAddresses.Remove(address);

            if (address.IsDefault)
            {
                var anotherAddress = await _context.UserAddresses
                    .FirstOrDefaultAsync(a => a.UserId == userId && a.AddressId != addressId);

                if (anotherAddress != null)
                {
                    anotherAddress.IsDefault = true;
                    anotherAddress.UpdatedAt = DateTime.Now;
                    _context.UserAddresses.Update(anotherAddress);
                }
            }

            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction("Checkout", "Order");
        }
    }
}