using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IUsersRepository
    {
        public void AddUser(string UserId);
        public User? GetFullUser(string UserId);
        public User? GetUser(string UserId);
        public bool UpdateUser(User user);
	}
}
