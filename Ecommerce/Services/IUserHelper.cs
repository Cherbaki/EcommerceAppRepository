using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IUserHelper
    {
        //Uses the cookies and userRepository to return the full user and it returns the full user
        public User? GetCurrentUser(HttpRequest request, HttpResponse response);
        public string? CreateAndGetUserId(HttpRequest request);
    }
}
