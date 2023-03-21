﻿using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.AspNetCore.Mvc;
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

        public int? GetStockQuantity(int productId)
        {
            var targetProduct = _dbContext.Products?.Find(productId);

            return targetProduct?.StockQuantity;
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
