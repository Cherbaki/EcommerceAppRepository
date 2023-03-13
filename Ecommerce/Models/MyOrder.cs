using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
	public class MyOrder
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(50)]
		public string? Email { get; set; } = String.Empty;
		[Required]
		public bool IsApproved { get; set; } = false;
		[Required]
		public DateTime CreationTime { get; set; } = DateTime.Now;
		[MaxLength(20)]
		public string? PaymentType { get; set; }



		public ShippingAddress? ShippingAddress { get; set; }
		public ICollection<MyItem>? MyItems { get; set; }

		[ForeignKey("User")]
		public string? UserId { get; set; }
		public User? User { get; set; }
	}
}
