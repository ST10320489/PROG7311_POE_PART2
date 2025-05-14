using Agri_Energy.Models;
using Microsoft.AspNetCore.Mvc;
using Supabase;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Text;
using System.Security.Cryptography;


namespace Agri_Energy.Controllers
{
    public class LoginController : Controller
    {

        private readonly Supabase.Client _supabase;

        public LoginController()
        {
            // initialize Supabase client (Microsoft, 2025)
            var options = new Supabase.SupabaseOptions();
            _supabase = new Supabase.Client("https://fpfvdrpoemvkwahfndrd.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZwZnZkcnBvZW12a3dhaGZuZHJkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDY3MzAyMTMsImV4cCI6MjA2MjMwNjIxM30.7cxDdcN1TtK-EQX4ZWRN-h8NMrTlfNpR9aumnvyOUvQ", options);
            _supabase.InitializeAsync().Wait();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // fetch the user by email (Microsoft, 2025)
            var user = await _supabase
                .From<User>()
                .Where(u => u.Email == model.Email)
                .Get();

            var foundUser = user.Models.FirstOrDefault();

            if (foundUser == null)
            {
                Console.WriteLine("No user found with the provided email.");
                ModelState.AddModelError("", "Email does not exist.");
                return View(model);
            }

            Console.WriteLine("User found. Comparing passwords...");

            // hash the entered password
            var hashedPassword = HashPassword(model.Password);

            Console.WriteLine($"{hashedPassword} compared to {foundUser.Password}");
            if (foundUser.Password != hashedPassword)
            {
                ModelState.AddModelError("", "Invalid password.");
                return View(model);
            }


            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, foundUser.Name),
        new Claim(ClaimTypes.Email, foundUser.Email),
        new Claim("Role", foundUser.Role),
        new Claim("UserId", foundUser.UserId.ToString())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            
            //Cookies to keep signed in user
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Home");
        }

        //to unhash the password we can compare (Microsoft, 2025)
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                // Convert password to bytes using UTF8 encoding
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);

                // Convert the hash to base64 to store/compare
                return Convert.ToBase64String(hash);
            }
        }


        //logging out method (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }
    }
}
