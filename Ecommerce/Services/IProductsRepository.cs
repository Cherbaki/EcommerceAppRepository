using Ecommerce.Models;

namespace Ecommerce.Services
{
	public interface IProductsRepository
	{
		public IEnumerable<Product>? GetProductsFully();
		public Task<Product?> GetFullProductByIdAsync(int productId);
		public int? GetStockQuantity(int productId);
		public bool ValidateQuantityAgainstStockQuantity(int productId, int givenQuantity);
		public Product? GetProductById(int productId);
		public bool UpdateProduct(Product product);
	}
}
