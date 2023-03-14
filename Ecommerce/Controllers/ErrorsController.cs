using Ecommerce.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.Controllers
{
	public class ErrorsController : Controller
	{
		public IActionResult ErrorPage(ErrorViewModel VM) => View(VM);
	}
}
