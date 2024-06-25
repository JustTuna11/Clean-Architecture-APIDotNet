using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Domain.Entities;
using WebAPI.Domain.InterfaceRepositories;
using WebAPI.Infrastructure.DataContext;

namespace WebAPI.Infrastructure.ImplementRepositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }
        #region Xử lý chuỗi
        private Task<bool> CompareStringAsync(string str1, string str2)
        {
            return Task.FromResult(string.Equals(str1.ToLowerInvariant(), str2.ToLowerInvariant()));
        }
        private async Task<bool> IsStringInListAsync(string inputString, List<string>listString)
        {
            if(inputString == null)
            {
                throw new ArgumentNullException(nameof(inputString));
            }
            if(listString == null)
            {
                throw new ArgumentNullException(nameof(listString));
            }
            foreach(var item in listString)
            {
                if(await CompareStringAsync(inputString,item))
                {
                       return true;
                }
            }
            return false;
        }
        #endregion
        // Thêm nhiều quyền cho 1 user
        public async Task AddRolesToUserAsync(User user, List<string> listRoles)
        {
            if (user == null) 
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (listRoles == null)
            {
                throw new ArgumentNullException(nameof(listRoles));
            }
            foreach (var roleCode in listRoles.Distinct()) 
                // duyệt các quyền đang muốn thêm
            {
                var roleOfUser = await GetRolesOfUserAsync(user); // Xem các quyền hiện đang có của user đó
                if(await IsStringInListAsync(roleCode, roleOfUser.ToList()))
                {
                    throw new ArgumentException("Người dùng đã có quyền này rồi");
                }
                else
                {
                    var roleItem = await _context.Roles.SingleOrDefaultAsync(x => x.RoleCode.Equals(roleCode));
                    if(roleItem == null)
                    {
                        throw new ArgumentNullException("Không tồn tại quyền này");
                    }
                    _context.Permissions.Add(new Permission
                    {
                        RoleId = roleItem.Id,
                        UserId = user.Id,
                    });
                }

            }
            _context.SaveChanges();
        }

        public async Task<IEnumerable<string>> GetRolesOfUserAsync(User user)
        {
            var roles = new List<string>();
            var listRolesOfUser = _context.Permissions.Where(x=>x.UserId == user.Id).AsQueryable();
            foreach(var item in listRolesOfUser)
            {
                var role = _context.Roles.SingleOrDefault(x => x.Id == item.RoleId);
                roles.Add(role.RoleCode);
            }
            return roles.AsEnumerable();
        }

        public async Task<User> GetUserbyEmail(string email)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Email.ToLower().Equals(email.ToLower()));
            return user;
        }

        public async Task<User> GetUserbyPhone(string phoneNumber)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.PhoneNumber.ToLower().Equals(phoneNumber.ToLower()));
            return user;
        }

        public async Task<User> GetUserbyUserName(string userName)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName.ToLower().Equals(userName.ToLower()));
            return user;
        }
    }
}
