using System.Linq;
using System.Threading.Tasks;
using Agri_Energy.Models;
using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace Agri_Energy.Controllers
{
    public class UsersController : Controller
    {
        private readonly Supabase.Client _supabase;

        public UsersController()
        {
            // Initialize your Supabase client (Microsoft, 2025)
            var options = new Supabase.SupabaseOptions();
            _supabase = new Supabase.Client("https://fpfvdrpoemvkwahfndrd.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZwZnZkcnBvZW12a3dhaGZuZHJkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDY3MzAyMTMsImV4cCI6MjA2MjMwNjIxM30.7cxDdcN1TtK-EQX4ZWRN-h8NMrTlfNpR9aumnvyOUvQ", options);
            _supabase.InitializeAsync().Wait();
        }

        public async Task<IActionResult> Index(string search)
        {
            var usersQuery = _supabase.From<User>();
            var usersResponse = await usersQuery.Get();

            var allUsers = usersResponse.Models;

            if (!string.IsNullOrEmpty(search))
            {
                allUsers = allUsers
                    .Where(u => u.Name.Contains(search, StringComparison.OrdinalIgnoreCase)
                             || u.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var viewModel = new UsersViewModel
            {
                Users = allUsers,
                SearchQuery = search
            };

            return View(viewModel);
        }
    }
}


