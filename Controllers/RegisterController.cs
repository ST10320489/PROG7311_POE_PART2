using Microsoft.AspNetCore.Mvc;
using Agri_Energy.Models;
using Supabase.Postgrest;
using System.Threading.Tasks;
using Supabase.Postgrest.Exceptions;

namespace Agri_Energy.Controllers
{
    public class RegisterController : Controller
    {
        private readonly Supabase.Client _supabase;

        public RegisterController()
        {
            // Initialize Supabase client (Microsoft, 2025)
            var options = new Supabase.SupabaseOptions();
            _supabase = new Supabase.Client("https://fpfvdrpoemvkwahfndrd.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZwZnZkcnBvZW12a3dhaGZuZHJkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDY3MzAyMTMsImV4cCI6MjA2MjMwNjIxM30.7cxDdcN1TtK-EQX4ZWRN-h8NMrTlfNpR9aumnvyOUvQ", options);
            _supabase.InitializeAsync().Wait();
        }


        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //hasing the password (Microsoft, 2025)
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Convert password to bytes using UTF8 encoding (Microsoft, 2025)
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);

                // Convert the hash to base64 to store/compare (Microsoft, 2025)
                return Convert.ToBase64String(hash);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Index(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    Name = model.Name,
                    Surname = model.Surname,
                    Email = model.Email,
                    Password = HashPassword(model.Password),
                    Role = model.Role,
                    Location = model.Location
                };

                try
                {
                    var response = await _supabase.From<User>().Insert(user);
                    TempData["SuccessMessage"] = "Registration successful!";
                    return RedirectToAction("Index", "Home");
                }
                catch (PostgrestException ex)
                {
                    // Check for duplicate email constraint
                    if (ex.Message.Contains("duplicate key") || ex.Message.Contains("Email"))
                    {
                        TempData["ErrorMessage"] = "An account with this email already exists.";
                        return RedirectToAction("Index"); // Goes back to form
                    }

                    // Other database error
                    TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
                    return RedirectToAction("Index");
                }
            }

            return View(model);
        }

    }
}
