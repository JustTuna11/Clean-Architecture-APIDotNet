using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Domain.Entities;

namespace WebAPI.Domain.InterfaceRepositories
{
    public interface IUserRepository
    {
        Task<User> GetUserbyEmail(string email);
        Task<User> GetUserbyUserName(string userName);
        Task<User> GetUserbyPhone(string phoneNumber);
        Task AddRolesToUserAsync(User user, List<string> listRoles);
        Task<IEnumerable<string>> GetRolesOfUserAsync(User user);
    }
}
