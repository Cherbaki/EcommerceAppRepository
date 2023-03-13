using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Models
{
	public class MyImage
	{
		[Key]
		public int Id { get; set; }
		[Required]
		[MaxLength(100)]
		public string? Name { get; set; }
		[Required]
		[MaxLength(100)]
		public string? ImageURL { get; set; }
		

		[ForeignKey("Product")]
		public int? ProductId { get; set; }
		public Product? Product { get; set; }
	}
}
