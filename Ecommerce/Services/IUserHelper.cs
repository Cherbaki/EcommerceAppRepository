using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IUserHelper
    {
        public User? GetCurrentUser(HttpRequest request, HttpResponse response);
    }
}
