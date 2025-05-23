﻿using Cloudzy.Data;
using Cloudzy.Models.Domain;
using Cloudzy.Models.ViewModels;
using Cloudzy.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
namespace Cloudzy.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly DbCloudzyContext _context;
        public UserRepository(DbCloudzyContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }
        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
        public async Task LockUserAsync(int id, bool lockStatus)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                user.IsLocked = lockStatus;
                await _context.SaveChangesAsync();
            }
        }
    }
}