using System;
namespace Agri_Energy.Models
{

	public class BlogViewModel
	{
        public List<Post> Posts { get; set; }
        public List<User> Farmers { get; set; }
        public string SearchQuery { get; set; }
    }
}

