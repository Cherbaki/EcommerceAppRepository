using Ecommerce.Models;

namespace Ecommerce.Services
{
    public interface IPaymentRepository
    {
        public Task<bool> AddOrderAsync(MyOrder newOrder);
        public bool UpdateMyItem(MyItem item);
        public bool UpdateUser(User user);
        public Task<MyOrder?> GetOrderByIdAsync(int orderId);
        public Task<bool> AddShippingAddressAsync(ShippingAddress shippingAddress);
        public Task<bool> UpdateMyOrder(MyOrder myOrder);
        public Task<MyOrder?> GetOrderWithItemsByIdAsync(int orderId);

	}
}
