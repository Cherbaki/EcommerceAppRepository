
using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class UsersRepository : IUsersRepository
    {
        private readonly AppDbContext _dbContext;        


        public UsersRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        
        public void AddUser(string UserId)
        {
            var newUser = new User { Id = UserId };

            _dbContext.Users?.Add(newUser);
            _dbContext.SaveChanges();
        }
        public User? GetFullUser(string UserId)
        {
            var targetUser = _dbContext.Users?
                                .Include(us => us.MyOrders)
                                .FirstOrDefault(us => us.Id == UserId);
            if (targetUser == null)
                return null;

            return targetUser;
        }

    }
}
