using Ecommerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.ViewModels
{
    public class ProductPageVM
    {
        public Product Product { get; set; }
        [Required]
        [Range(1,1000)]
        public int SelectedQuantity { get; set; } = 1;
        [Required]
        public string? Caller { get; set; }
    }
}
