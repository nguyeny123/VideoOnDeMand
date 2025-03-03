﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoOnDemand.Admin.Models;
using VideoOnDemand.Data.Data;
using VideoOnDemand.Data.Data.Entities;

namespace VideoOnDemand.Admin.Services
{
    public class UserService : IUserService
    {
        private VODContext _db;
        private readonly UserManager<User> _userManager;
        public UserService(VODContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IEnumerable<UserPageModel> GetUsers()
        {
            return from user in _db.Users
                   orderby user.Email
                   select new UserPageModel
                   {
                       Id = user.Id,
                       Email = user.Email,
                       IsAdmin = _db.UserRoles.Any(ur =>
                       ur.UserId.Equals(user.Id) &&
                       ur.RoleId.Equals(1.ToString()))
                   };
        }
        public UserPageModel GetUser(string userId)
        {
            return (from user in _db.Users
                    where user.Id.Equals(userId)
                    select new UserPageModel
                    {
                        Id = user.Id,
                        Email = user.Email,
                        IsAdmin = _db.UserRoles.Any(ur =>
                        ur.UserId.Equals(user.Id) &&
                        ur.RoleId.Equals(1.ToString()))
                    }).FirstOrDefault();
        }
        public async Task<IdentityResult> AddUserAsync(RegisterUserPageModel user)
        {
            var dbUser = new User
            {
                UserName = user.Email,
                Email = user.Email,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(dbUser, user.Password);
            return result;
        }
        public async Task<bool> UpdateUser(UserPageModel user)
        {
            var dbUser = await _db.Users.FirstOrDefaultAsync(u =>
            u.Id.Equals(user.Id));
            if (dbUser == null) return false;
            if (string.IsNullOrEmpty(user.Email)) return false;
            dbUser.Email = user.Email;
            var userRole = new IdentityUserRole<string>()
            {
                RoleId = "1",
                UserId = user.Id
            };
            var isAdmin = await _db.UserRoles.AnyAsync(ur =>
            ur.Equals(userRole));
            if (isAdmin && !user.IsAdmin)
                _db.UserRoles.Remove(userRole);
            else if (!isAdmin && user.IsAdmin)
                await _db.UserRoles.AddAsync(userRole);
            var result = await _db.SaveChangesAsync();
            return result >= 0;
        }
        public async Task<bool> DeleteUser(string userId)
        {
            try
            {
                var dbUser = await _db.Users.FirstOrDefaultAsync(d =>
                d.Id.Equals(userId));
                if (dbUser == null) return false;
                var userRoles = _db.UserRoles.Where(ur =>
                ur.UserId.Equals(dbUser.Id));
                _db.UserRoles.RemoveRange(userRoles);
                _db.Users.Remove(dbUser);
                var result = await _db.SaveChangesAsync();
                if (result < 0) return false;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
