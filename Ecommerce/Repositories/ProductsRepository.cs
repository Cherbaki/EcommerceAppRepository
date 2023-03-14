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

        public async Task<Product?> GetFullProductById(int productId)
        {
            return await _dbContext.Products!
                            .Include(pr => pr.MyImages)!
                            .FirstOrDefaultAsync(pr => pr.Id == productId);
        }

        public int? GetStockQuantity(int productId)
        {
            var targetProduct = _dbContext.Products?.Find(productId);

            return targetProduct?.StockQuantity;
        }

    }
}
