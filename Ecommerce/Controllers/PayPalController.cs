using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
using PayPalCheckoutSdk.Orders;

namespace Ecommerce.Controllers
{
	public class PayPalController : Controller
	{
		private readonly IConfiguration _configuration;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IProductsRepository _productsRepository;
		private readonly IUsersRepository _usersRepository;

		private readonly string? _payPalEnvironment;
		private readonly string? _clientId;
		private readonly string? _clientSecret;

		private MyPayPalSetup _myPayPalSetup;


		public PayPalController(IConfiguration configuration, AppDbContext dbContext, 
			IPaymentRepository paymentRepository, IProductsRepository productsRepository,IUsersRepository usersRepository)
		{
			_configuration = configuration;
			_paymentRepository = paymentRepository;
			_productsRepository = productsRepository;
			_usersRepository = usersRepository;

			_payPalEnvironment = configuration["Paypal:payPalEnvironment"];
			_clientId = configuration["Paypal:clientId"];
			_clientSecret = configuration["Paypal:clientSecret"];


			_myPayPalSetup = new MyPayPalSetup()
			{
				Environment = _payPalEnvironment,
				ClientId = _clientId,
				Secret = _clientSecret
			};
		}

		public async Task<IActionResult> PayPalPay(MyOrder? newOrder = null, string? info = null)
		{
			if (newOrder == null)
				return NotFound("Order not found");

			//If info doesn't equal to null so it's call from the paypal
			if (info != null)
			{
				return RedirectToAction("PayPalPayRedirectAction",
											new
											{
												newOrderId = newOrder.Id,
												info = info,
												payerId = Request.Query["PayerID"]!,
												payerApprovedOrderId = Request.Query["token"]
											});
			}

			newOrder = await _paymentRepository.GetOrderWithItemsByIdAsync(newOrder.Id);

			if (newOrder == null)
			{
				var errorViewModel = new ErrorViewModel()
				{
					Message = "Order is not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorViewModel);
			}
			if (newOrder.MyItems == null)
			{
				var errorViewModel = new ErrorViewModel()
				{
					Message = "Order items is not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorViewModel);
			}


			_myPayPalSetup.RedirectUrl = Request.Scheme + "://" + Request.Host + Request.Path;
			var response = await PayPalHelpers.CreateOrder(_myPayPalSetup, newOrder.MyItems!.ToList());

			var Statuse = response.StatusCode;
			var result = response.Result<Order>();

			foreach (LinkDescription link in result.Links)
			{
				Console.WriteLine("\t{0}: {1}\tCall Type: {2}", link.Rel, link.Href, link.Method);
				if (link.Rel.Trim().ToLower() == "approve")
				{
					_myPayPalSetup.ApproveUrl = link.Href;
				}
			}

			if (!string.IsNullOrEmpty(_myPayPalSetup.ApproveUrl))
				return Redirect(_myPayPalSetup.ApproveUrl);


			var errorVM = new ErrorViewModel()
			{
				Message = "Order is not approved"
			};

			return RedirectToAction("ErrorPage", "Errors", errorVM);
		}
		public async Task<IActionResult> PayPalPayRedirectAction(int? newOrderId = null, string info = "", string payerId = ""
														, string payerApprovedOrderId = "")
		{
			//Check Payment Cancelation
			if (!string.IsNullOrEmpty(info) && info.Trim().ToLower() == "cancel")
				return RedirectToAction("OrderResult", "Payment", new { info = "Canceled" });

			if (newOrderId == null)
				return NotFound("Order not found");

			MyOrder? newOrder = null;
			if (info.Trim().ToLower() == "capture")
				newOrder = await _paymentRepository.GetFullOrderByIdAsync(newOrderId.Value);
			if (newOrder == null)
			{
				var errorVM = new ErrorViewModel()
				{
					Message = "Order not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}
			if (string.IsNullOrEmpty(payerId))
			{
				var errorVM = new ErrorViewModel()
				{
					Message = "PayerId is not found"
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

			_myPayPalSetup.PayerApprovedOrderId = payerApprovedOrderId;

			var response = await PayPalHelpers.CaptureOrder(_myPayPalSetup);
			try
			{
				var statusCode = response!.StatusCode;
				Order result = response.Result<Order>();

				//If everything went successfull approving order,Decreasy the stock quantities and clear up the Cart, if needed.
				foreach (var item in newOrder.MyItems!)
				{
					var targetProduct = _productsRepository.GetProductById(item.ProductId!.Value);

					if (targetProduct == null)
					{
						var errorVM = new ErrorViewModel()
						{
							Message = "Items product is not found"
						};

						return RedirectToAction("ErrorPage", "Errors", errorVM);
					}
						
					//Decrease the value of the quantity(Quantity is already validated and no need to check the result)
					targetProduct.StockQuantity -= int.Parse(item.Quantity!);

					_productsRepository.UpdateProduct(targetProduct);
				}

				//Clear up the user Cart
				var userId = Request.Cookies["UserId"];
				if(userId == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "User Id is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}
				var targetUser = _usersRepository.GetFullUser(userId);
				if (targetUser == null)
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "User is not found"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				var cartItems = targetUser.MyItems?.Where(mi => mi.IsInCart).ToList();
				//Lets find out if the user purchased for the cart items or for individual item
				bool purchasedCartItems = false;
				if (newOrder.PurchaseOption == "CheckOut")
				{
					purchasedCartItems = true;
				}
				else if (newOrder.PurchaseOption == "Buy")
					purchasedCartItems = false;
				else
				{
					var errorVM = new ErrorViewModel()
					{
						Message = "Purchase option is not recognized"
					};

					return RedirectToAction("ErrorPage", "Errors", errorVM);
				}

				if(cartItems != null) { 
					//If the user purchased the cart items then clear the cart
					if (purchasedCartItems) { 
						//Delete every item from items table associated with this particular users cart
						//Some of them has asociated orders as well, which are actual approved orders items
						//And they should not be deleted
						foreach (var item in cartItems)
							_paymentRepository.DeleteItem(item);
						_paymentRepository.SaveChangesInTheDatabase();
					}
				}

				newOrder.IsApproved = true;
				//Attach new approved order to the user
				newOrder.UserId = targetUser.Id;
				await _paymentRepository.UpdateMyOrderAsync(newOrder);
				
			}
			catch (Exception ex)
			{
				var errorVM = new ErrorViewModel()
				{
					Message = ex.Message
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}


			return RedirectToAction("OrderResult", "Payment", new { info = "Approved", orderId = newOrder.Id });
		}


	}
}
