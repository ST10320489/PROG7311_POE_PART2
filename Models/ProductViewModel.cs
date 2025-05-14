using System;
namespace Agri_Energy.Models
{
	public class ProductViewModel
	{
        public List<Product> Product { get; set; }
        public List<User> Farmers { get; set; }
        public string SearchQuery { get; set; }
    }
}

