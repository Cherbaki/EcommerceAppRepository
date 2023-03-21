using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
	public class GetShippingAddressVM
	{
		[Required]
		[MaxLength(20)]
		public string? FirstName { get; set; }
		[Required]
		[MaxLength(20)]
		public string? LastName { get; set; }
		[Required]
		[MaxLength(50)]
		public string? Address { get; set; }
		[Required]
		[MaxLength(50)]
		public string? Country { get; set; }
		[Required]
		[MaxLength(50)]
		public string? City { get; set; }
		[Required]
		[MaxLength(10)]
		public string? PostCode { get; set; }
		[Required]
		[MaxLength(50)]
		public string? Email { get; set; }



		public int OrderId { get; set; }
	}
}
