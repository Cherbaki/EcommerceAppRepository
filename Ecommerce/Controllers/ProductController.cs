using Ecommerce.Data;
using Ecommerce.Helpers;
using Ecommerce.Models;
using Ecommerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Drawing;

namespace Ecommerce.Controllers
{
	public class ProductController : Controller
	{
		private readonly AppDbContext _dbContext;
		private readonly IWebHostEnvironment _webHostEnvironment;


        public ProductController(AppDbContext dbContext,IWebHostEnvironment webHostEnvironment)
        {
			_dbContext = dbContext;
			_webHostEnvironment = webHostEnvironment;

		}


		[HttpGet]
        public IActionResult Index()
		{


			return View();
		}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Index(IndexVM VM)
		{

			AddImageToTheProduct(VM.Files!);

			return RedirectToAction("Index");
		}


		public void AddImageToTheProduct(IFormFileCollection files)
		{
			var targetProduct = _dbContext.Products?.Find(5);


			foreach(var file in files)
			{
				var newImage = new MyImage()
				{
					Name = file.FileName,
					ImageURL = FileHelpers.GetImageUrl("Images/", file, _webHostEnvironment),
					ProductId = targetProduct?.Id
				};

				_dbContext.MyImages?.Add(newImage);
			}

			_dbContext.SaveChanges();

		}


	}
}
