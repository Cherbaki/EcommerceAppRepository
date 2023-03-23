using Ecommerce.Models;

namespace Ecommerce.ViewModels
{
	public class OrderResultVM
	{
		public string? Information { get; set; }
		public bool Approved { get; set; }
		public MyOrder? ApprovedOrder { get; set; }
	}
}
