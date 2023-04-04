using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Repositories
{
    public class ProductsRepository : IProductsRepository
    {
        private readonly AppDbContext _dbContext;


        public ProductsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public IEnumerable<Product>? GetProductsFully()
        {
            return _dbContext.Products?.Include(pr => pr.MyImages);
        }
        public async Task<Product?> GetFullProductByIdAsync(int productId)
        {
            return await _dbContext.Products!
                            .Include(pr => pr.MyImages)!
                            .FirstOrDefaultAsync(pr => pr.Id == productId);
        }
        public Product? GetProductById(int productId)
        {
            return _dbContext.Products?.Find(productId);
        } 
        public int? GetStockQuantity(int productId)
        {
            var targetProduct = _dbContext.Products?.Find(productId);

            return targetProduct?.StockQuantity;
        }
        public bool UpdateProduct(Product product)
        {
            try
            {
                _dbContext.Products!.Update(product);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public IEnumerable<Product>? GetRelatedProducts(Product targetProdct)
        {
            //This methid should be returning related products of the given product
            var relatedProduct = _dbContext.Products?.Where(pr => pr.Id != targetProdct.Id).Take(4);

            return relatedProduct;
        }


        //We have an Action with an identical implementation which is used by the front-end,
        //When this one is used by the backend to validate the quantity validity at different states
        //Of the payment
		public bool ValidateQuantityAgainstStockQuantity(int productId, int givenQuantity)
		{
			var stockQuantity = GetStockQuantity(productId);
            if(stockQuantity != null && givenQuantity <= stockQuantity)
				return true;
			return false;
		}

	}
}
