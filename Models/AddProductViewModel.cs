using System;
namespace Agri_Energy.Models
{
    public class AddProductViewModel
    {
        public List<ProductType> ProductTypes { get; set; } = new List<ProductType>();
        public Product Product { get; set; }
    }
}
