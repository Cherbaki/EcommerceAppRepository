using Ecommerce.Models;

namespace Ecommerce.Services
{
	public interface IProductsRepository
	{
		public IEnumerable<Product>? GetProductsFully();
		public Task<Product?> GetFullProductById(int productId);
		public int? GetStockQuantity(int productId);

    }
}
