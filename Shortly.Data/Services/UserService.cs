using Microsoft.EntityFrameworkCore;
using Shortly.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shortly.Data.Services
{
    public class UserService:IUserService
    {
        private AppDbContext _context;

        public  UserService(AppDbContext context)
        {

            _context = context;

        }

        public async Task<User> GetUserbyId(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            return user;
        }
        public async Task<List<User>> GetUsers()
        {
            var allusers = await _context.Users.Include(y => y.Urls).ToListAsync();
            return allusers;

        }


        public async Task<User >AddUser(User user)
        {
           await  _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;

        }

        public async Task<User> UpdateUser(string id, User user)
        {
            var userdb = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userdb != null)
            {
                userdb.Email = user.Email;
                //userdb.Address = user.Address;
                //userdb.Name = user.Name;

                await _context.SaveChangesAsync();

            
            }
            return userdb;
        
        }


        public async Task DeleteUser (string id)
        {
            var userdb = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            bool del = false;
            if (userdb != null)
            {
                _context.Users.Remove(userdb);

                _context.SaveChangesAsync();
                del = true;


            }
            //return del;

        }
    }
}
