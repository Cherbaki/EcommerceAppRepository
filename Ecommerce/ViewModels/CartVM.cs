using Ecommerce.Models;

namespace Ecommerce.ViewModels
{
    public class CartVM
    {
        public required User CurrentUser { get; set; }
        public List<MyItem>? CartItems { get; set; }
    }
}
