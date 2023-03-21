using Ecommerce.Models;
using Ecommerce.Services;

namespace Ecommerce.Helpers
{
    public class UserHelper : IUserHelper
    {
        private readonly IUsersRepository _usersRepository;


        public UserHelper(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }


        //If the user doesn't exist it'll create one
        public User? GetCurrentUser(HttpRequest request, HttpResponse response)
        {
            var userId = CheckAndGetUserId(request, response);

            return _usersRepository.GetFullUser(userId);
        }
        public string? CreateAndGetUserId(HttpRequest request)
        {
            string? userId = request.Cookies["UserId"];

            return userId;
        }


        private string CheckAndGetUserId(HttpRequest request,HttpResponse response)
        {
            var userId = CreateAndGetUserId(request); 
            if (userId == null)
            {
                userId = SaveCookiesForUserId(response);
            }

            return userId;
        }
        private string SaveCookiesForUserId(HttpResponse response)
        {
            var options = new CookieOptions();
            options.Expires = DateTime.Now.AddYears(1);
            var newId = Guid.NewGuid().ToString();

            response.Cookies.Append("UserId", newId, options);


            //Now let's also create and save the new user in the database
            _usersRepository.AddUser(newId);

            return newId;
        }

    }
}
