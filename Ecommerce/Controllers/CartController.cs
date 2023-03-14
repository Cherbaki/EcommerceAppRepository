using Ecommerce.Helpers;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly IUserHelper _userHelper;


        public CartController(IUserHelper userHelper)
        {
            _userHelper = userHelper;
        }
         

        public IActionResult Cart()
        {
            //Gets/Saves user from/in the database.
            var currentUser = _userHelper.GetCurrentUser(Request,Response);



            return View();
        }
        
        
    }
}
