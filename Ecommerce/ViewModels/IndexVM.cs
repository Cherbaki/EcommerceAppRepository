using Ecommerce.Models;

namespace Ecommerce.ViewModels
{
	public class IndexVM
	{
		public required IEnumerable<Product> Products { get; set; }
	}
}
