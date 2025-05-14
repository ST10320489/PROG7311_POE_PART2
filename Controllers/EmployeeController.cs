using Microsoft.AspNetCore.Mvc;
using Agri_Energy.Models;
using Supabase;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Agri_Energy.ViewModels;

namespace Agri_Energy.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly Supabase.Client _supabase;

        public EmployeeController()
        {
            // Initialize your Supabase client (Microsoft, 2025)
            var options = new Supabase.SupabaseOptions();
            _supabase = new Supabase.Client("https://fpfvdrpoemvkwahfndrd.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZwZnZkcnBvZW12a3dhaGZuZHJkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDY3MzAyMTMsImV4cCI6MjA2MjMwNjIxM30.7cxDdcN1TtK-EQX4ZWRN-h8NMrTlfNpR9aumnvyOUvQ", options);
            _supabase.InitializeAsync().Wait();
        }

        // Action to display the list of all the Users from the database (Microsoft, 2025)
        public async Task<IActionResult> Users(string search)
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

            return View("Users/Index", viewModel); // returns
        }


        // GET: User/Add (Microsoft, 2025)
        [HttpGet]
        public IActionResult Add()
        {
            return View("Users/Add");
        }


        //hasing the password
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

        // POST: User/Add (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> Add(User user)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the form errors and try again.";
                return View("Users/Add", user);
            }

            try
            {
                // Hash the password before storing it
                user.Password = HashPassword(user.Password);

                await _supabase.From<User>().Insert(user);
                TempData["Success"] = "User added successfully!";
                return RedirectToAction("Users", "Employee");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while adding the user. Please try again.";
                return View("Users/Add", user);
            }
        }



        // GET: User/Edit (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _supabase.From<User>().Where(u => u.UserId == id).Get(); //Get user by id
            var user = response.Models.FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }

            return View("Users/Edit", user);
        }


        // POST: User/Edit (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> Edit(User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please correct the form errors and try again.";
                return View("Users/Edit", updatedUser);
            }

            // Hash the password before updating
            updatedUser.Password = HashPassword(updatedUser.Password);

            await _supabase.From<User>().Update(updatedUser);
            TempData["Success"] = "User updated successfully!";
            return RedirectToAction("Users", "Employee");
        }



        // POST: User/Delete POPUP (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Get user
            var response = await _supabase.From<User>().Where(u => u.UserId == id).Get(); //Get user by id 
            var user = response.Models.FirstOrDefault();

            if (user != null)
            {
                // Delete related products
                var productsResponse = await _supabase.From<Product>().Where(p => p.FarmerId == id).Get();
                var userProducts = productsResponse.Models;
                foreach (var product in userProducts)
                {
                    await _supabase.From<Product>().Delete(product);
                }

                // Delete related posts
                var postsResponse = await _supabase.From<Post>().Where(p => p.UserId == id).Get();
                var userPosts = postsResponse.Models;
                foreach (var post in userPosts)
                {
                    await _supabase.From<Post>().Delete(post);
                }

                // Now delete the user
                await _supabase.From<User>().Delete(user);

                TempData["Success"] = "User and all related data deleted successfully!";
            }

            return RedirectToAction("Users", "Employee");
        }






        //POST
        //GET: Post/Index (Microsoft, 2025)
        //Returns a list of all the posts to from the database
        [HttpGet]
        public async Task<IActionResult> Posts(string search)
        {
            var postsResponse = await _supabase.From<Post>().Get();
            var allPosts = postsResponse.Models;

            var usersResponse = await _supabase.From<User>().Get();
            var allUsers = usersResponse.Models;

            if (!string.IsNullOrEmpty(search))
            {
                // Get matching users based on email
                var matchingUserIds = allUsers
                    .Where(u => u.Email.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .Select(u => u.UserId)
                    .ToList();

                //Search function
                // Filter posts by title/content or by matching userId 
                allPosts = allPosts
                    .Where(p => p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                p.Content.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                matchingUserIds.Contains(p.UserId))
                    .ToList();
            }

            var viewModel = new BlogViewModel
            {
                Posts = allPosts,
                Farmers = allUsers,
                SearchQuery = search
            };

            return View("Posts/Index", viewModel);
        }


        //GET: Post/Add (Microsoft, 2025)
        [HttpGet]
        public IActionResult AddPost()
        {
            return View("~/Views/Farmer/Post/Add.cshtml", new AddPostViewModel());
        }


        //POST: Post/Add (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> AddPost(AddPostViewModel model)
        {
            Console.WriteLine("Entered AddPost method.");



            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine($"User email: {email}");

            if (string.IsNullOrEmpty(email))
            {
                Console.WriteLine("Email is null or empty. Redirecting to login.");
                return RedirectToAction("Index", "Login");
            }

            var employeeQuery = await _supabase.From<User>().Where(x => x.Email == email).Get();
            var employee = employeeQuery.Models.FirstOrDefault();

            if (employee == null)
            {
                Console.WriteLine("Employee not found.");
                return NotFound("Employee not found.");
            }

            var newPost = model.Post;
            newPost.UserId = employee.UserId;
            newPost.CreatedAt = DateTime.UtcNow;
            newPost.UpdatedAt = DateTime.UtcNow;

            Console.WriteLine($"Post Data: Title = {newPost.Title}, Content = {newPost.Content}, Category = {newPost.Category}, UserId = {newPost.UserId}");

            var file = Request.Form.Files["imageFile"];
            if (file != null && file.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                Console.WriteLine($"Uploading image: {fileName}");

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var options = new Supabase.Storage.FileOptions
                    {
                        ContentType = file.ContentType
                    };

                    await _supabase.Storage
                        .From("agri-photos")
                        .Upload(memoryStream.ToArray(), fileName, options);
                }

                newPost.ImageUrl = $"https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/{fileName}";
            }
            else
            {
                Console.WriteLine("No image uploaded. Using default image.");
                newPost.ImageUrl = "https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/default_image.webp";
            }

            try
            {
                Console.WriteLine("Attempting to insert post into Supabase...");
                var response = await _supabase.From<Post>().Insert(newPost);

                Console.WriteLine($"Insert response: Status Code = {response.ResponseMessage.StatusCode}");

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error inserting post: {response.ResponseMessage.Content.ReadAsStringAsync().Result}");
                    TempData["Error"] = "There was a problem adding the post.";
                    return View("~/Views/Farmer/Post/Add.cshtml", model);
                }

                Console.WriteLine("Post added successfully.");
                TempData["Success"] = "Post added successfully!";
                return RedirectToAction("Posts", "Employee");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while inserting post: {ex.Message}");
                TempData["Error"] = $"Failed to add post: {ex.Message}";
                return View("~/Views/Farmer/Post/Add.cshtml", model);
            }
        }




        // [GET]: Post/Edit (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> EditPost(int id)
        {

            var postQuery = await _supabase.From<Post>().Where(x => x.PostId == id).Get();
            var post = postQuery.Models.FirstOrDefault();

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            var viewModel = new AddPostViewModel
            {
                Post = post
            };

            viewModel.Post.ImageUrl = post.ImageUrl;

            // Reuse of other view
            return View("~/Views/Farmer/Post/Edit.cshtml", viewModel);
        }


        // [POST]: Post/Edit (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> EditPost(AddPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                //Reuse other view
                return View("~/Views/Farmer/Post/Edit.cshtml", model);
            }

            var existingPostQuery = await _supabase.From<Post>()
                .Where(x => x.PostId == model.Post.PostId).Get();

            var existingPost = existingPostQuery.Models.FirstOrDefault();

            if (existingPost == null)
            {
                TempData["Error"] = "Post not found.";
                return RedirectToAction("EditPost", new { id = model.Post.PostId });
            }

            // Update post fields
            existingPost.Title = model.Post.Title;
            existingPost.Content = model.Post.Content;
            existingPost.Category = model.Post.Category;

            // Optional image update
            var file = Request.Form.Files["imageFile"];
            if (file != null && file.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;

                    var options = new Supabase.Storage.FileOptions
                    {
                        ContentType = file.ContentType
                    };

                    await _supabase.Storage
                        .From("agri-photos")
                        .Upload(memoryStream.ToArray(), fileName, options);
                }

                existingPost.ImageUrl = $"https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/{fileName}";
            }

            // Save updates
            var response = await _supabase.From<Post>().Update(existingPost);

            if (response.Models.Any())
            {
                TempData["Success"] = "Post updated successfully!";
                return RedirectToAction("Posts", "Employee"); 
            }

            TempData["Error"] = "Update failed.";
            return RedirectToAction("EditPost", new { id = model.Post.PostId });
        }


        // [POST]: Post/Delete (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> DeletePost(int id)
        {
            var postQuery = await _supabase.From<Post>().Where(p => p.PostId == id).Get();
            var post = postQuery.Models.FirstOrDefault();

            if (post == null)
            {
                return NotFound("Post not found.");
            }

            //Delete images from storages
            var imageUrl = post.ImageUrl;
            var storagePath = imageUrl?.Split("/agri-photos/").Last();

            if (!string.IsNullOrEmpty(storagePath) && !imageUrl.Contains("default_image.webp"))
            {
                await _supabase.Storage
                    .From("agri-photos")
                    .Remove(new List<string> { storagePath });
            }
            else
            {
                Console.WriteLine("[DELETE] No custom image to delete.");
            }

            await _supabase
                .From<Post>()
                .Where(p => p.PostId == id)
                .Delete();

            TempData["Success"] = "Post deleted.";
            return RedirectToAction("Posts", "Employee");
        }


        // GET: Post/PostDetails (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> PostDetails(int id)
        {
            var postResponse = await _supabase
                .From<Post>()
                .Filter("post_id", Supabase.Postgrest.Constants.Operator.Equals, id)
                .Get();

            var post = postResponse.Models.FirstOrDefault();

            if (post == null)
                return NotFound("Post not found");

            var userResponse = await _supabase
                .From<User>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, post.UserId)
                .Get();

            var user = userResponse.Models.FirstOrDefault();

            var viewModel = new PostDetailsViewModel
            {
                Post = post,
                User = user
            };

            return View("Posts/PostDetails", viewModel);
        }







        //PRODUCTS
        // GET: Product/Index
        [HttpGet]
        public async Task<IActionResult> Products(string search)
        {
            var productQuery = _supabase.From<Product>();
            var productResponse = await productQuery.Get();
            var allProducts = productResponse.Models;

            var farmerResponse = await _supabase.From<User>().Get();
            var allFarmers = farmerResponse.Models;

            if (!string.IsNullOrEmpty(search))
            {
                allProducts = allProducts
                    .Where(p =>
                        p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        allFarmers.FirstOrDefault(f => f.UserId == p.FarmerId)?.Email.Contains(search, StringComparison.OrdinalIgnoreCase) == true
                    ).ToList();
            }

            var viewModel = new ProductViewModel
            {
                Product = allProducts,
                Farmers = allFarmers,
                SearchQuery = search
            };

            return View("Products/Index", viewModel); // make sure this is the correct view path
        }


        // GET: Farmer/Product/AddProduct
        [HttpGet]
        public async Task<IActionResult> Add1()
        {
            var productTypesResponse = await _supabase.From<ProductType>().Get();

            var viewModel = new AddProductViewModel
            {
                ProductTypes = productTypesResponse.Models
            };

            return View("~/Views/Farmer/Product/Add.cshtml", viewModel); // Reuse the same view
        }


        // POST: Farmer/Product/AddProduct
        [HttpPost]
        public async Task<IActionResult> Add1(AddProductViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login");
            }

            var farmerQuery = await _supabase.From<User>().Where(x => x.Email == email).Get();
            var farmer = farmerQuery.Models.FirstOrDefault();

            if (farmer == null)
            {
                return NotFound("Farmer not found.");
            }

            var newProduct = model.Product;

            if (newProduct == null)
            {
                return BadRequest("Product data is missing.");
            }

            newProduct.DateListed = DateTime.UtcNow;
            newProduct.Status = model.Product.Status;
            newProduct.FarmerId = farmer.UserId;
            model.Product.ImageUrl = "default";

            // Handle file upload
            try
            {
                var file = Request.Form.Files["imageFile"];
                Console.WriteLine(file != null ? $"File uploaded: {file.FileName}" : "No file uploaded.");

                if (file != null && file.Length > 0)
                {
                    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        memoryStream.Position = 0;

                        var options = new Supabase.Storage.FileOptions
                        {
                            ContentType = file.ContentType
                        };

                        var fileBytes = memoryStream.ToArray();
                        await _supabase.Storage
                            .From("agri-photos")
                            .Upload(fileBytes, fileName, options);

                        var imageUrl = $"https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/{fileName}";
                        newProduct.ImageUrl = imageUrl;
                        Console.WriteLine($"Image uploaded and URL set: {imageUrl}");
                    }
                }
                else
                {
                    newProduct.ImageUrl = "https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/default_image.webp";
                    Console.WriteLine("Default image URL used.");
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("ModelState is invalid. Listing errors:");
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            Console.WriteLine($"Field: {state.Key} - Error: {error.ErrorMessage}");
                        }
                    }

                    model.ProductTypes = (await _supabase.From<ProductType>().Get()).Models;
                    return View("~/Views/Farmer/Product/Add.cshtml", model);

                }

                var response = await _supabase.From<Product>().Insert(newProduct);
                Console.WriteLine("Product inserted into database.");

                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("Products", "Employee");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Products", "Employee");
            }

        }


        // POST: Product/Delete POPUP
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // Get the product by ID
            var productQuery = await _supabase.From<Product>().Where(p => p.ProductId == id).Get();
            var product = productQuery.Models.FirstOrDefault();

            if (product == null)
            {
                return NotFound("Product not found.");
            }

            // Extract the filename from the ImageUrl
            var imageUrl = product.ImageUrl;
            var storagePath = imageUrl?.Split("/agri-photos/").Last();

            // Only delete image if it's not the default one
            if (!string.IsNullOrEmpty(storagePath) && !imageUrl.Contains("default_image.webp"))
            {
                await _supabase.Storage
                    .From("agri-photos")
                    .Remove(new List<string> { storagePath });
            }

            await _supabase
                .From<Product>()
                .Where(p => p.ProductId == id)
                .Delete();

            return RedirectToAction("Products");
        }


        // GET: Product/ProductDetail
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            // Get the product by ID
            var productResponse = await _supabase
                .From<Product>()
                .Filter("product_id", Supabase.Postgrest.Constants.Operator.Equals, id)
                .Get();

            var product = productResponse.Models.FirstOrDefault();

            if (product == null)
            {
                return NotFound(); 
            }

            // Get the farmer who posted the product
            var farmerResponse = await _supabase
                .From<User>()
                .Filter("user_id", Supabase.Postgrest.Constants.Operator.Equals, product.FarmerId)
                .Get();

            var farmer = farmerResponse.Models.FirstOrDefault();

            // 3. Pass to view
            var viewModel = new ProductDetailsViewModel
            {
                Product = product,
                Farmer = farmer
            };

            return View("Products/ProductDetails", viewModel);

        }

    }
}
