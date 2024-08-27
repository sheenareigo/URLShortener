using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
        Task<User> GetUserbyId(string id);

        Task<User> AddUser(User user);
        Task<User> UpdateUser(string id, User user);

        Task DeleteUser(string id);
    }
}
