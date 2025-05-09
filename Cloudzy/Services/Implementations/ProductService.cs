﻿using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminProduct;
using Cloudzy.Models.ViewModels.Product;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductService(IProductRepository productRepository, IWebHostEnvironment webHostEnvironment)
        {
            _productRepository = productRepository;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task AddAsync(CreateViewModel model)
        {
            //Kiểm tra trùng tên sản phẩm
            var existingProduct = (await _productRepository.GetAllAsync())
                .FirstOrDefault(p => p.ProductName == model.ProductName);
            if (existingProduct != null)
            {
                throw new Exception("Tên sản phẩm đã tồn tại");
            }

            if (model.Images == null)
            {
                throw new Exception("Bạn cần phải tải lên một file ảnh.");
            }

            // Lưu ảnh vào thư mục wwwroot/images
            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }

            // Xử lý lưu ảnh
            string fileName = Path.GetFileNameWithoutExtension(model.Images.FileName);
            string extension = Path.GetExtension(model.Images.FileName);
            string uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
            string filePath = Path.Combine(uploadFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await model.Images.CopyToAsync(fileStream);
            }

            var product = new Product
            {
                ProductName = model.ProductName,
                CategoryId = model.CategoryId,
                BrandId = model.BrandId,
                SupplierId = model.SupplierId,
                Material = model.Material,
                Price = model.Price ?? 0,
                DiscountPrice = model.DiscountPrice ?? 0,
                ProductDescription = model.ProductDescription,
                ProductImages = new List<ProductImage>
                {
                    new ProductImage { ImageUrl = "/images/" + uniqueFileName }
                }
            };

            await _productRepository.AddAsync(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) throw new ArgumentException("Sản phẩm không tồn tại.");

            // Xóa ảnh sản phẩm khỏi thư mục
            foreach (var img in product.ProductImages)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            await _productRepository.DeleteAsync(id);
        }

        public async Task<IPagedList<ListViewModel>> GetAllAsync(int pageNumber, int pageSize)
        {
            var products = await _productRepository.GetAllAsync();
            return products.Select((p, index) => new ListViewModel
            {
                ProductId = p.ProductId,
                STT = index + 1,
                ProductName = p.ProductName,
                CategoryName = p.Category?.CategoryName ?? "Không có",
                BrandName = p.Brand?.BrandName ?? "Không có",
                Material = p.Material,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                ProductImages = p.ProductImages.Select(img => img.ImageUrl).ToList()
            }).ToPagedList(pageNumber, pageSize);
        }

        public async Task<EditViewModel> GetByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new EditViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryId = product.CategoryId,
                BrandId = product.BrandId,
                SupplierId = product.SupplierId,
                Material = product.Material,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                ProductDescription = product.ProductDescription,
                CurrentImages = product.ProductImages.Select(img => img.ImageUrl).ToList()
            };
        }

        public async Task<IPagedList<ProductListViewModel>> GetFilteredProductsAsync(int pageNumber, int pageSize, int? categoryId, int? brandId, string? searchTerm)
        {
            var products = await _productRepository.GetFilteredProductsAsync(categoryId, brandId, searchTerm);
            return products.Select(p => new ProductListViewModel
            {
                ProductId = p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                Material = p.Material,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.CategoryName,
                BrandId = p.BrandId,
                BrandName = p.Brand?.BrandName,
                ProductImages = p.ProductImages.Select(pi => pi.ImageUrl).ToList()
            }).ToPagedList(pageNumber,pageSize);
        }

        public async Task UpdateAsync(EditViewModel model)
        {
            //Kiểm tra trùng tên sản phẩm
            var existingProduct = (await _productRepository.GetAllAsync())
                .FirstOrDefault(p => p.ProductName == model.ProductName && p.ProductId != model.ProductId);
            if (existingProduct != null)
            {
                throw new Exception("Tên sản phẩm đã tồn tại");
            }

            var product = await _productRepository.GetByIdAsync(model.ProductId);
            if (product == null)
            {
                throw new Exception("Sản phẩm không tồn tại.");
            }

            //Kiểm tra tất cả các ảnh bị chọn xóa và không thêm ảnh mới
            bool hasRemainingImages = model.CurrentImages != null && model.CurrentImages.Count > 0;
            bool hasNewImage = model.NewImages != null;
            if(!hasRemainingImages && !hasNewImage)
            {
                throw new Exception("Giữ lại ít nhất 1 ảnh hoặc thêm ảnh mới");
            }

            product.ProductName = model.ProductName;
            product.CategoryId = model.CategoryId ?? product.CategoryId;
            product.BrandId = model.BrandId ?? product.BrandId;
            product.SupplierId = model.SupplierId;
            product.Material = model.Material ?? product.Material;
            product.Price = model.Price ?? product.Price;
            product.DiscountPrice = model.DiscountPrice ?? product.DiscountPrice;
            product.ProductDescription = model.ProductDescription;

            string uploadFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");

            // Xóa ảnh không còn tồn tại trong CurrentImages
            var imagesToDelete = product.ProductImages
                .Where(img => !model.CurrentImages.Contains(img.ImageUrl))
                .ToList();

            foreach (var img in imagesToDelete)
            {
                string filePath = Path.Combine(_webHostEnvironment.WebRootPath, img.ImageUrl.TrimStart('/'));
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                product.ProductImages.Remove(img);
            }

            // Xử lý thêm ảnh mới
            if (model.NewImages != null)
            {
                string fileName = Path.GetFileNameWithoutExtension(model.NewImages.FileName);
                string extension = Path.GetExtension(model.NewImages.FileName);
                string uniqueFileName = $"{fileName}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                string filePath = Path.Combine(uploadFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.NewImages.CopyToAsync(fileStream);
                }

                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = "/images/" + uniqueFileName
                });
            }

            await _productRepository.UpdateAsync(product);
        }
    }
}
