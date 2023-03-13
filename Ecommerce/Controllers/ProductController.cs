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


	}
}
