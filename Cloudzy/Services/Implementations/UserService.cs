﻿using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels.AdminUser;
using Cloudzy.Repositories.Interfaces;
using Cloudzy.Services.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using X.PagedList.Extensions;

namespace Cloudzy.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly DbCloudzyContext _context;

        public UserService(IUserRepository userRepository, DbCloudzyContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<IPagedList<ListViewModel>> GetAllUsersAsync(int pageNumber, int pageSize)
        {
            var users = await _userRepository.GetAllUsersAsync();
            var pagedUsers = users.Select((u, index) => new ListViewModel
            {
                UserId = u.UserId,
                STT = index + 1,
                Email = u.Email,
                Fullname = u.Fullname,
                PhoneNumber = u.PhoneNumber,
                Address = u.Address,
                RoleName = u.Role?.RoleName ?? "N/A",
                IsLocked = u.IsLocked
            }).ToPagedList(pageNumber, pageSize);

            return pagedUsers;
        }

        public async Task<EditViewModel?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return null;

            return new EditViewModel
            {
                UserId = user.UserId,
                Fullname = user.Fullname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                RoleId = user.RoleId ?? 0,
                Roles = new SelectList(_context.Roles, "RoleId", "RoleName")
            };
        }

        public async Task AddUserAsync(CreateViewModel model)
        {
            var existingEmail = (await _userRepository.GetAllUsersAsync())
                .FirstOrDefault(u => u.Email == model.Email);

            if (existingEmail != null)
            {
                throw new Exception("Email đã tồn tại!");
            }

            var user = new User
            {
                Fullname = model.Fullname,
                Email = model.Email,
                Password = model.Password,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                RoleId = model.RoleId
            };

            await _userRepository.AddUserAsync(user);
        }

        public async Task UpdateUserAsync(EditViewModel model)
        {
            var user = await _userRepository.GetUserByIdAsync(model.UserId);
            if (user == null) return;

            user.Fullname = model.Fullname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.RoleId = model.RoleId;

            await _userRepository.UpdateUserAsync(user);
        }

        public async Task LockUnlockUserAsync(int id)
        {
            var user = await _userRepository.GetUserByIdAsync(id);
            if (user == null) return;

            bool newLockStatus = !user.IsLocked;

            await _userRepository.LockUserAsync(id, newLockStatus);
        }
    }
}