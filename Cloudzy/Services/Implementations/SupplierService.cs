﻿using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminSupplier;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        public SupplierService(ISupplierRepository supplierRepository)
        {
            _supplierRepository = supplierRepository;
        }
        public async Task AddAsync(CreateViewModel model)
        {
            //Kiểm tra trùng nhà cung cấp
            var existingSupplier = (await _supplierRepository.GetAllAsync())
                .FirstOrDefault(s => s.Email == model.Email);
            if (existingSupplier != null)
            {
                throw new Exception("Nhà cung cấp đã tồn tại");
            }

            var supplier = new Supplier
            {
                SupplierName = model.SupplierName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address
            };
            await _supplierRepository.AddAsync(supplier);
        }

        public async Task DeleteAsync(int id)
        {
            await _supplierRepository.DeleteAsync(id);
        }

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            var pageSuppliers = suppliers.Select((s, index) => new ListViewModel
            {
                SupplierId = s.SupplierId,
                STT = index + 1,
                SupplierName = s.SupplierName,
                Email = s.Email,
                PhoneNumber = s.PhoneNumber,
                Address = s.Address
            }).ToPagedList(pageNumber, pageSize);
            return pageSuppliers;
        }

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier == null) return null;

            return new EditViewModel
            {
                SupplierId = supplier.SupplierId,
                SupplierName = supplier.SupplierName,
                Email = supplier.Email,
                PhoneNumber = supplier.PhoneNumber,
                Address = supplier.Address
            };
        }

        public async Task UpdateAsync(EditViewModel model)
        {

            var supplier = await _supplierRepository.GetByIdAsync(model.SupplierId);
            if (supplier == null)
            {
                throw new Exception("Nhà cung cấp không tồn tại");
            }

            //Kiểm tra trùng nhà cung cấp
            var existingSupplier = (await _supplierRepository.GetAllAsync())
                .FirstOrDefault(s => s.Email == model.Email && s.SupplierId != model.SupplierId);
            if (existingSupplier != null)
            {
                throw new Exception("Nhà cung cấp đã tồn tại");
            }

            supplier.SupplierName = model.SupplierName;
            supplier.Email = model.Email;
            supplier.PhoneNumber = model.PhoneNumber;
            supplier.Address = model.Address;

            await _supplierRepository.UpdateAsync(supplier);
        }
    }
}
