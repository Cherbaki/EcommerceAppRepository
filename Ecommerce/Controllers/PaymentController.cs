using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Ecommerce.Controllers
{
    public class PaymentController : Controller
    {
        private readonly IProductsRepository _productsRepository;
        private readonly IUsersRepository _usersRepository;
        private readonly IPaymentRepository _paymentRepository;


        public PaymentController(IProductsRepository productsRepository, IUsersRepository usersRepository, IPaymentRepository paymentRepository)
        {
            _productsRepository = productsRepository;
            _usersRepository = usersRepository;
            _paymentRepository = paymentRepository;
        }


        [HttpGet]
        public async Task<IActionResult> CreateItem(int productId, int quantity)
        {
            //Get the product from the database
            var targetProduct = await _productsRepository.GetFullProductByIdAsync(productId);
            if (targetProduct == null)
                return NotFound("Product is not found in the database!!!");

            MyItem? NewItem = null;
            //Attach item to the user
            NewItem = new MyItem()
            {
                Quantity = quantity.ToString(),
                Name = targetProduct.Name,
                CurrencyCode = "USD",
                TaxValue = "0",
                UnitAmountValue = targetProduct.CurrentPrice,
                Sku = "",
                ShippingValue = "0",
                ProductId = productId
            };

            string? userId = Request.Cookies["UserId"];
            //If we have an User
            if (Request.Cookies["UserId"] != null)
            {
                var targetUser = _usersRepository.GetFullUser(userId!);

                if (targetUser == null)
                    return NotFound("Target User Not Found");

                //Attach item to the user
                NewItem.UserId = targetUser.Id;
            }
            else
            {
                var errorVM = new ErrorViewModel()
                {
                    Message = "User is not found"
                };

                return RedirectToAction("ErrorPage", "Errors", errorVM);
            }

            var items = new List<MyItem>() { NewItem };

            //Creating not approved order, Shipping address and email is assigned when the order is captured
            var newOrder = new MyOrder()
            {
                MyItems = items,
                PurchaseOption = "Buy"
            };

            NewItem.MyOrder = newOrder;
            NewItem.MyOrderId = newOrder.Id;


            await _paymentRepository.AddOrderAsync(newOrder);

            return RedirectToAction("GetShippingAddress", new { orderId = newOrder.Id });
        }
        [HttpGet]
        public async Task<IActionResult> AddItemToCart(int productId, int quantity)
        {
            try {
                //Get the product from the database
                var targetProduct = await _productsRepository.GetFullProductByIdAsync(productId);
                if (targetProduct == null)
                {
                    var errorVM = new ErrorViewModel()
                    {
                        Message = "Selected product is not found"
                    };

                    return RedirectToAction("ErrorPage", "Errors", errorVM);
                }

                var cartItem1 = new MyItem()
                {
                    Quantity = quantity.ToString(),
                    Name = targetProduct?.Name,
                    CurrencyCode = "USD",
                    TaxValue = "0",
                    UnitAmountValue = targetProduct?.CurrentPrice,
                    Sku = "",
                    ShippingValue = "0",
                    ProductId = productId,
                    IsInCart = true
                };

                string? userId = Request.Cookies["UserId"];
                if (userId != null)
                {
                    var targetUser = _usersRepository.GetFullUser(userId);
                    if (targetUser == null)
                    {
                        var errorVM = new ErrorViewModel()
                        {
                            Message = "User is not found"
                        };

                        return RedirectToAction("ErrorPage", "Errors", errorVM);
                    }

                    //Let's check if this kind of item is already in the user cart
                    MyItem? sameItem = targetUser.MyItems?
                                            .Where(it => it.IsInCart)
                                            .FirstOrDefault(it => it.ProductId == cartItem1.ProductId);

                    if (sameItem != null)
                    {
                        //There is same kind of item.
                        int sameItemQuanitity, cartItem1Quanitity;
                        try
                        {
                            sameItemQuanitity = int.Parse(sameItem.Quantity!);
                            cartItem1Quanitity = int.Parse(cartItem1.Quantity);
                        }
                        catch (Exception ex)
                        {
                            var errorVM = new ErrorViewModel()
                            {
                                Message = ex.Message
                            };

                            return RedirectToAction("ErrorPage", "Errors", errorVM);
                        }

                        int productStockQuantity = targetProduct!.StockQuantity!.Value;

                        //If possible just increment the quantitiy of that item
                        if (sameItemQuanitity + cartItem1Quanitity <= productStockQuantity)
                        {
                            //Just increase the quanitity of the item which is in the cart
                            sameItem.Quantity = (sameItemQuanitity + cartItem1Quanitity).ToString();

                            _paymentRepository.UpdateMyItem(sameItem);
                        }
                        else//if not display the error message
                        {
                            if (sameItemQuanitity <= 0)
                            {
                                TempData["PaymentErrorMessage"] = "There is not enough item in the stock";

                                return RedirectToAction("ProductPage", "Product",
                                    new {
                                        productId = sameItem.ProductId
                                    });
                            }


                            return RedirectToAction("ProductPage", "Product",
                                   new { productId = sameItem.ProductId, message = "There is not enough item in the stock. You already have some in the cart" });
                        }

                    }
                    else
                    {
                        //We have an user,which doesn't have this kind of item in the cart
                        cartItem1.UserId = targetUser.Id;
                        targetUser.MyItems!.Add(cartItem1);
                        cartItem1.UserId = targetUser.Id;


                        _paymentRepository.UpdateUser(targetUser);
                    }
                }
                else
                {
                    var errorVM = new ErrorViewModel()
                    {
                        Message = "User is not found"
                    };

                    return RedirectToAction("ErrorPage", "Errors", errorVM);
                }

                return RedirectToAction("ProductPage", "Product", new { productId = productId, message = "Item added in the cart" });
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
        [HttpGet]
        public async Task<IActionResult> CheckOut()
        {
            var userId = Request.Cookies["UserId"];
            if (userId == null)
            {
                var errorVM = new ErrorViewModel()
                {
                    Message = "User is not found"
                };

                return RedirectToAction("ErrorPage", "Errors", errorVM);
            }

            var targetUser = _usersRepository.GetFullUser(userId);

            if (targetUser == null)
                return NotFound("Target User Not Found");

            /*
                Creating copy list of new items(New rows are not generated because of same IDs)
            If the item has an associated order, then it's already approved orders item
            Which is not a cart item(But it's just associated with this user)
            */
            var items = new List<MyItem>();

            var cartItems = targetUser.MyItems?.Where(mi => mi.IsInCart);

            //If there are no items in the cart return in the main page
            if (cartItems == null || cartItems.Count() == 0)
                return RedirectToAction("Index", "");

            foreach (var item in cartItems)
            {
                items.Add(new MyItem
                {
                    Quantity = item.Quantity,
                    Name = item.Name,
                    CurrencyCode = item.CurrencyCode,
                    TaxValue = item.TaxValue,
                    UnitAmountValue = item.UnitAmountValue,
                    Sku = "",
                    ShippingValue = item.ShippingValue,
                    ProductId = item.ProductId,
                    UserId = item.UserId,
                    IsInCart = false
                });
            }

            //Creating not approved order, Shipping address and email is assigned when the order is captured
            var newOrder = new MyOrder()
            {
                MyItems = items,
                PurchaseOption = "CheckOut"
            };
            foreach (var item in newOrder.MyItems)
            {
                item.MyOrderId = newOrder.Id;
            }

            //Once this order will be captured, we'll clear up users cart from the items(which say that IsInCart=true)
            //If the order won't be captured we'll leave them inside
            await _paymentRepository.AddOrderAsync(newOrder);


            return RedirectToAction("GetShippingAddress", new { orderId = newOrder.Id });
        }
        [HttpGet]
        public IActionResult GetShippingAddress(int orderId)
        {
            var gsaVM = new GetShippingAddressVM()
            {
                OrderId = orderId
            };


            return View(gsaVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GetShippingAddress(GetShippingAddressVM gsaVM)
        {
            //Validate the view model
            if (gsaVM == null) { 
			    var errorVM = new ErrorViewModel()
			    {
				    Message = "Shipping address is not found"
				};
			    return RedirectToAction("ErrorPage", "Errors", errorVM);
            }
            if (gsaVM?.OrderId == null) { 
				var errorVM = new ErrorViewModel()
				{
					Message = "Current Order Id is not found"
				};
				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

            var targetOrder = await _paymentRepository.GetOrderByIdAsync(gsaVM.OrderId);
            if (targetOrder == null)
                return NotFound("Order can't be null");


            string emailRegex = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";

            if (!Regex.IsMatch(gsaVM.Email!, emailRegex))
            {
				var errorVM = new ErrorViewModel()
				{
					Message = "Email is not in the right format"
				};
				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

            var newShippingAddress = new ShippingAddress()
            {
                FirstName = gsaVM.FirstName,
                LastName = gsaVM.LastName,
                Address = gsaVM.Address,
                City = gsaVM.City,
                Country = gsaVM.Country,
                PostCode = gsaVM.PostCode,
                MyOrderId = gsaVM.OrderId//It won't be null it's already validated
            };

            //Update the Email field of the targetOrder
            targetOrder.Email = gsaVM.Email;
            targetOrder.ShippingAddress = newShippingAddress;

            await _paymentRepository.AddShippingAddressAsync(newShippingAddress);
            await _paymentRepository.UpdateMyOrderAsync(targetOrder);


            return RedirectToAction("ChoosePaymentType", new { orderId = targetOrder.Id });
        }
		[HttpGet]
		public IActionResult ChoosePaymentType(int? orderId)
		{
			if (orderId == null)
			{
				var errorVM = new ErrorViewModel()
				{
					Message = "Order is not found"
				};
				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

			return View(orderId);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ChoosePaymentType(int? orderId, string? paymentType)
		{
            try
            {
                //Validate the orderId
                if (orderId == null)
                {
                    var errorVM = new ErrorViewModel()
                    {
                        Message = "Order id is not found"
                    };
                    return RedirectToAction("ErrorPage", "Errors", errorVM);
                }
                var targetOrder = await _paymentRepository.GetOrderWithItemsByIdAsync(orderId.Value);
                if (targetOrder == null)
                {
                    var errorVM = new ErrorViewModel()
                    {
                        Message = "Order not found"
                    };
                    return RedirectToAction("ErrorPage", "Errors", errorVM);
                }

                //Check if all the items quantity in the order is suitable for the stock Quantity
                //Because during retrieving user shipping address, items might be sold
                foreach(var item in targetOrder.MyItems!)
                {
                    var result = _productsRepository.ValidateQuantityAgainstStockQuantity(item.ProductId!.Value, int.Parse(item.Quantity!));
                    if (!result)
					{
                        return RedirectToAction("ProductPage", "Product",
                            new { productId = item.ProductId, message = "Specified quantity is not valid for this item"}
                        );
                    }

                }

                //Now validate the payment type(For now we only have PayPal option)
                if (paymentType != "PayPalPay")
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "Payment type is not found"
					};
					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				targetOrder.PaymentType = paymentType;

                await _paymentRepository.UpdateMyOrderAsync(targetOrder);

                if (paymentType == "PayPalPay")
                    return RedirectToAction(paymentType, "PayPal", targetOrder);

                //Here we can add conditions for other payment types
                return NotFound("Selected Payment type is not found");
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

		[HttpGet]
		public async Task<IActionResult> OrderResult(string info, int orderId)
		{
			if (info == "")
			{
				var errorVM = new ErrorViewModel()
				{
					Message = "Orders result lacks the information!"
				};
				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}


            var targetOrder = await _paymentRepository.GetFullOrderByIdAsync(orderId);
            if(targetOrder == null && info != "Canceled")
            {
				var errorVM = new ErrorViewModel()
				{
					Message = "Order is not found!"
				};
				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

			var newVM = new OrderResultVM()
			{
				Information = info,
				ApprovedOrder = targetOrder,   
			};

			if (info == "Canceled" && targetOrder == null)
				newVM.Approved = false;
			else
			{
				newVM.Approved = true;

				//_emailHelper.SendOrderEmail(orderId);
			}


			return View(newVM);
		}

	}
}
