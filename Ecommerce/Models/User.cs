using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
	public class User
	{
		[Key]
		[Required]
		public string? Id { get; set; }


		public ICollection<MyOrder>? MyOrders { get; set; }
		public ICollection<MyItem>? MyItems { get; set; }
	}
}
