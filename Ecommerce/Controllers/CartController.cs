using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
    public class CartController : Controller
    {
        private readonly IUserHelper _userHelper;
        private readonly IPaymentRepository _paymentRepository;


		public CartController(IUserHelper userHelper, IPaymentRepository paymentRepository)
        {
            _userHelper = userHelper;
            _paymentRepository = paymentRepository;
        }


        [HttpGet]
        public IActionResult Cart(string? quantityValidityMessage)
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
                CartItems = currentUser.MyItems?.Where(it => it.IsInCart).ToList(),
				QuantityValidityMessage = quantityValidityMessage
			};

            return View(VM);
        }

		[HttpGet]
		public IActionResult RemoveMyItemFromTheCart(int? myItemId)
		{
			if (myItemId == null)
            {
				var errorVM = new ErrorViewModel()
				{
					Message = "Item to delte is not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

			var currentUser = _userHelper.GetCurrentUser(Request,Response);
            if(currentUser == null)
            {
                var errorVM = new ErrorViewModel()
                {
                    Message = "User is not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

            var targetItem = currentUser.MyItems?.FirstOrDefault(my => my.IsInCart && my.Id == myItemId);
			if (targetItem == null)
            {
				var errorVM = new ErrorViewModel()
				{
					Message = "Item to delte is not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}


            _paymentRepository.DeleteItem(targetItem);

			return RedirectToAction("Cart");
		}

		[HttpGet]
		public IActionResult IncrementMyItemQuantity(int? myItemId)
		{
			try {
				if (myItemId == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "Item is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				//Get the item from the cart
				var currentUser = _userHelper.GetCurrentUser(Request, Response);
				if (currentUser == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "User is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				var targetItem = currentUser.MyItems?.FirstOrDefault(my => my.IsInCart && my.Id == myItemId);
				if (targetItem == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "Item to delte is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}


				//Increment the quantitiy
				MyItem? updatedItem = targetItem;
				int quantity = int.Parse(targetItem.Quantity!);
				quantity++;
				updatedItem.Quantity = quantity.ToString();


				//Check if that many item is available in the stock and if not give as much as you can
				var result = _paymentRepository.CheckAndChangeMyItemsQuantityFromStock(ref targetItem);
				string? messageAboutItemQuantity = null;
				if (!result.wasValid && result.message == "Not Available")
					messageAboutItemQuantity = "This item is no longer available in the stock.";
				else if (!result.wasValid && result.message == "Decreased")
					messageAboutItemQuantity = "Quantity has modified, since there is not enough item in the stock";


				targetItem.Quantity = updatedItem.Quantity;

				_paymentRepository.UpdateMyItem(targetItem);


				return RedirectToAction("Cart", new { quantityValidityMessage = messageAboutItemQuantity });
			}
			catch(Exception ex)
			{
				var errorVM = new ErrorViewModel()
				{
					Message = ex.Message
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}
		}
		[HttpGet]
		public IActionResult DecrementMyItemQuantity(int? myItemId)
		{
			try
			{
				if (myItemId == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "Item is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				//Get the item from the cart
				var currentUser = _userHelper.GetCurrentUser(Request, Response);
				if (currentUser == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "User is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				var targetItem = currentUser.MyItems?.FirstOrDefault(my => my.IsInCart && my.Id == myItemId);
				if (targetItem == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "Item to delte is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}


				//Increment the quantitiy
				MyItem? updatedItem = targetItem;
				int quantity = int.Parse(targetItem.Quantity!);
				quantity--;
				updatedItem.Quantity = quantity.ToString();
				
				string? messageAboutItemQuantity = null;
				if (quantity <= 0)
				{
					messageAboutItemQuantity = "Number of items must be positive";
					targetItem.Quantity = 1.ToString();
				}
				else
				{
					//Check if that many item is available in the stock and if not give as much as you can
					var result = _paymentRepository.CheckAndChangeMyItemsQuantityFromStock(ref targetItem);
					if (!result.wasValid && result.message == "Not Available")
						messageAboutItemQuantity = "This item is no longer available in the stock.";
					else if (!result.wasValid && result.message == "Decreased")
						messageAboutItemQuantity = "Quantity has modified, since there is not enough item in the stock";
					
					targetItem.Quantity = updatedItem.Quantity;
				}

				_paymentRepository.UpdateMyItem(targetItem);

				return RedirectToAction("Cart", new { quantityValidityMessage = messageAboutItemQuantity });
			}
			catch (Exception ex)
			{
				var errorVM = new ErrorViewModel()
				{
					Message = ex.Message
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}
		}
	}
}
