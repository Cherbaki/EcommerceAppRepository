using Ecommerce.Helpers;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

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
            //Gets(if needed creates and Saves) user from the database.
            var currentUser = _userHelper.GetCurrentUser(Request,Response);
            if(currentUser == null)
            {
                var errorVM = new ErrorViewModel()
                {
                    Message = "User is not found"
                };

                return RedirectToAction("ErrorPage", "Errors", errorVM);
            }

            var VM = new CartVM
            {
                CurrentUser = currentUser,
                CartItems = currentUser.MyItems?.Where(it => it.IsInCart).ToList()
			};

            return View(VM);
        }
        
        
    }
}
