using Ecommerce.Data;
using Ecommerce.Models;
using Ecommerce.Services;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Ecommerce.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _dbContext;

        
        public PaymentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<bool> AddOrderAsync(MyOrder newOrder)
        {
            try { 
                await _dbContext.MyOrders!.AddAsync(newOrder);
                await _dbContext.SaveChangesAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateMyItem(MyItem item)
        {
            try
            {
                _dbContext.MyItems?.Update(item);
                _dbContext.SaveChanges();

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool UpdateUser(User user)
        {
            try
            {
                _dbContext.Users?.Update(user);
                _dbContext.SaveChanges();

                return true;
            }
            catch{
                return false;
            }
        }
        public async Task<MyOrder?> GetOrderByIdAsync(int orderId)
        {
            try
            {
                return await _dbContext.MyOrders!.FindAsync(orderId);
            }
            catch
            {
                return null;
            }
        }
        public async Task<MyOrder?> GetOrderWithItemsByIdAsync(int orderId)
        {
            try
            {
                return await _dbContext.MyOrders!
                                        .Include(or => or.MyItems)!
                                        .FirstOrDefaultAsync(or => or.Id == orderId);
            }
            catch
            {
                return null;
            }
        }
        public async Task<bool> AddShippingAddressAsync(ShippingAddress shippingAddress)
        {
            try { 
                await _dbContext.ShippingAddresses!.AddAsync(shippingAddress);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
		}
        public async Task<bool> UpdateMyOrderAsync(MyOrder myOrder)
        {
            try
            {
                _dbContext.MyOrders!.Update(myOrder);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<MyOrder?> GetFullOrderByIdAsync(int orderId)
        {
			try
			{
				return await _dbContext.MyOrders!
										.Include(or => or.MyItems)!
                                            .ThenInclude(it => it.Product)
                                                .ThenInclude(pr => pr!.MyImages)
                                        .Include(or => or.ShippingAddress)
										.FirstOrDefaultAsync(or => or.Id == orderId);
			}
			catch
			{
				return null;
			}
		}
        public bool DeleteItem(MyItem item)
        {
            try
            {
                _dbContext.MyItems?.Remove(item);
                _dbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public void SaveChangesInTheDatabase()
        {
            _dbContext.SaveChanges();
        }
		public (bool wasValid, string? message) CheckAndChangeMyItemsQuantityFromStock(ref MyItem item)
		{
			var targetProduct = _dbContext.Products?.Find(item.ProductId);

			if (targetProduct == null)
				throw new Exception("Product of the given item is not found");

			//Validating Quantity
			int? realStockQuantity = targetProduct.StockQuantity;

			if (realStockQuantity == null)
				throw new Exception("Product is not found in the stock");

			if (realStockQuantity <= 0)
			{
				item.Quantity = "0";
				return (false, "Not Available");
			}
			else if (realStockQuantity < int.Parse(item.Quantity!))
			{
				//At this point there are not enough items in the stock to satisfy the item
				//So we can just give as much as we can

				//Assign as much as available
				item.Quantity = realStockQuantity.ToString();

				return (false, "Decreased");
			}


			return (true, "");
		}
	}
}
