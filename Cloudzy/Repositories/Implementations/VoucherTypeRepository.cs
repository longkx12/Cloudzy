﻿using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cloudzy.Repositories.Implementations
{
    public class VoucherTypeRepository : IVoucherTypeRepository
    {
        private readonly DbCloudzyContext _context;
        public VoucherTypeRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task AddAsync(VoucherType entity)
        {
            await _context.VoucherTypes.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            //Kiểm tra xem có voucher nào áp dụng loại voucher cần xóa không
            bool hasVouchers = await _context.DiscountCodes.AnyAsync(d => d.VoucherTypeId == id);
            if (hasVouchers)
            {
                throw new InvalidOperationException("Không thể xóa vì có voucher đang sử dụng loại voucher này.");
            }

            var voucherType = await _context.VoucherTypes.FindAsync(id);
            if (voucherType != null)
            {
                _context.VoucherTypes.Remove(voucherType);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<VoucherType>> GetAllAsync()
        {
            return await _context.VoucherTypes.ToListAsync();
        }

        public async Task<VoucherType> GetByIdAsync(int id)
        {
            return await _context.VoucherTypes.FindAsync(id);
        }

        public async Task UpdateAsync(VoucherType entity)
        {
            _context.VoucherTypes.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
