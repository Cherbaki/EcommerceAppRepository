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
    }
}
