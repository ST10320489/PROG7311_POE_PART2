using System;
namespace Agri_Energy.Models
{
    public class FarmerProductViewModel
    {
        public List<Product> Products { get; set; }
        public List<ProductType> ProductTypes { get; set; }
        public string SearchTerm { get; set; }
    }
}

