using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IUsersRepository
    {
        public void AddUser(string UserId);
        public User? GetFullUser(string UserId);
    }
}
