using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Ecommerce.Models
{
	public class MyItem
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(40)]
		public string? Name { get; set; }//Should match name of the product
		[Required]
		[MaxLength(10)]
		public string? Quantity { get; set; }
		[Required]
		[MaxLength(10)]
		public string? CurrencyCode { get; set; }
		[Required]
		[MaxLength(20)]
		public string? TaxValue { get; set; }
		[Required]
		[MaxLength(20)]
		public string? UnitAmountValue { get; set; }
		[Required]
		[MaxLength(20)]
		public string? Sku { get; set; }
		[Required]
		[MaxLength(20)]
		public string? ShippingValue { get; set; }
		[Required]
		public bool IsInCart { get; set; } = false;


		[ForeignKey("MyOrder")]
		public int? MyOrderId { get; set; }
		public MyOrder? MyOrder { get; set; }

		[ForeignKey("User")]
		public string? UserId { get; set; }
		public User? User { get; set; }

		[ForeignKey("Product")]
		public int? ProductId { get; set; }
		public Product? Product { get; set; }
	}
}
