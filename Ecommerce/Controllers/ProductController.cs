using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol;

namespace Ecommerce.Controllers
{
	public class ProductController : Controller
	{
		private readonly IProductsRepository _productsRepository;
		private readonly IUserHelper _userHelper;


        public ProductController(AppDbContext dbContext, IProductsRepository productsRepository, IUserHelper userHelper)
        {
			_productsRepository = productsRepository;
			_userHelper = userHelper;
		}


		[HttpGet]
        public IActionResult Index()
		{
			var products = _productsRepository.GetProductsFully();
			if(products == null)
			{
				var errorVM = new ErrorViewModel()
				{
					Message = "There is not a product in the database"
				};

				return RedirectToAction("ErrorPage", "Errors", errorVM);
			}

			var VM = new IndexVM
			{
				Products = products
			};

			return View(VM);
		}
		[HttpGet]
		public async Task<IActionResult> ProductPage(int productId,string? message = null)
		{
			var targetProduct = await _productsRepository.GetFullProductByIdAsync(productId);
			if (targetProduct == null)
			{
                var errorVM = new ErrorViewModel()
                {
                    Message = "Product is not found in the database"
                };

                return RedirectToAction("ErrorPage", "Errors", errorVM);
            }

			//Gather related Products
			var relatedProduct = _productsRepository.GetRelatedProducts(targetProduct);


            var VM = new ProductPageVM
			{
				Product = targetProduct,
				ProductId = targetProduct.Id,
				MessageFromPayment = message,
				RelatedProducts = relatedProduct
            };			

            return View(VM);
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ProductPage(ProductPageVM VM)
		{
			try {
                //Posted result might have some problem and to return to the view we need the product again
                VM.Product = (await _productsRepository.GetFullProductByIdAsync(VM.ProductId))!;

                //Applay server side validation to the selected quantity
                var json = ValidateQuantityAgainstStockQuantity(VM.ProductId, VM.SelectedQuantity).ToJson().ToString();
				var result = json.Contains("true");
				if (!result)
				{
					VM.MessageFromPayment = "There is not enough item in the stock";

                    return View(VM);
                }

				if (!ModelState.IsValid)
					return View(VM);

				//If the user doesn't exist it'll create one
				_userHelper.GetCurrentUser(Request,Response);

				if (VM.Caller == "Buy")
					return RedirectToAction("CreateItem", "Payment", new { productId = VM.ProductId, quantity = VM.SelectedQuantity });
                
				return RedirectToAction("AddItemToCart", "Payment", new { productId = VM.ProductId, quantity = VM.SelectedQuantity });
            }
            catch
			{
                var errorVM = new ErrorViewModel()
                {
                    Message = "Errro occured while processing the specified quanity"
                };

                return RedirectToAction("ErrorPage", "Errors", errorVM);
            }

        }

		[HttpGet]//This action here is for the AJAX which uses Json to validate the selected quantity for the product
		public IActionResult ValidateQuantityAgainstStockQuantity(int productId,int givenQuantity)
		{
			var stockQuantity = _productsRepository.GetStockQuantity(productId);
            if (stockQuantity == null)
            {
                var errorVM = new ErrorViewModel()
                {
                    Message = "Product is not found in the database"
                };

                return RedirectToAction("ErrorPage", "Errors", errorVM);
            }


            if (givenQuantity > stockQuantity || givenQuantity <= 0)
				return Json(new { success = false });
			
			return Json(new { success = true });
        }
	}
}
