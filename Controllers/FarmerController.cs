using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Agri_Energy.Models;
using Agri_Energy.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Supabase;

namespace Agri_Energy.Controllers
{
    public class FarmerController : Controller
    {
        private readonly Supabase.Client _supabase;

        public FarmerController()
        {
            // initialize your Supabase client (Microsoft, 2025)
            var options = new Supabase.SupabaseOptions();
            _supabase = new Supabase.Client("https://fpfvdrpoemvkwahfndrd.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImZwZnZkcnBvZW12a3dhaGZuZHJkIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDY3MzAyMTMsImV4cCI6MjA2MjMwNjIxM30.7cxDdcN1TtK-EQX4ZWRN-h8NMrTlfNpR9aumnvyOUvQ", options);
            _supabase.InitializeAsync().Wait();
        }

        // ALL PRODUCTS 
        //[GET] : List of products (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> MarketPlace(string searchTerm, int? productTypeId, string category,
                                             double? minPrice, double? maxPrice,
                                             DateTime? startDate, DateTime? endDate)
        {
            // get all products (Microsoft, 2025)
            var productResponse = await _supabase.From<Product>().Get();
            var products = productResponse.Models;

            // Filter by name (Microsoft, 2025)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                products = products.Where(p =>
                    (!string.IsNullOrEmpty(p.Name) && p.Name.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(p.Description) && p.Description.ToLower().Contains(searchTerm))
                ).ToList();
            }

            // Filter by Product Type
            if (productTypeId.HasValue)
            {
                products = products.Where(p => p.ProductTypeId == productTypeId.Value).ToList();
            }

            // Filter by Price Range
            if (minPrice.HasValue)
            {
                products = products.Where(p => p.Price >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(p => p.Price <= maxPrice.Value).ToList();
            }

            // Filter by Date Range
            if (startDate.HasValue)
            {
                products = products.Where(p => p.DateListed.Date >= startDate.Value.Date).ToList();
            }
            if (endDate.HasValue)
            {
                products = products.Where(p => p.DateListed.Date <= endDate.Value.Date).ToList();
            }

            // Show message if no products match the filter
            if (!products.Any())
            {
                TempData["Info"] = "No products found for the selected filter.";
            }

            // dropdown data
            var productTypesResponse = await _supabase.From<ProductType>().Get();
            var farmersResponse = await _supabase.From<User>().Get();

            //ViewModel
            var viewModel = new MarketPlaceViewModel
            {
                Products = products,
                ProductTypes = productTypesResponse.Models,
                Farmers = farmersResponse.Models
            };

            return View("MarketPlace", viewModel);
        }

        //[GET] : Prdouct details (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var productResponse = await _supabase
                .From<Product>()
                .Filter("product_id", Supabase.Postgrest.Constants.Operator.Equals, id)
                .Get();

            var product = productResponse.Models.FirstOrDefault();

            if (product == null)
            {
                return NotFound(); 
            }

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

            return View("Details", viewModel);

        }







        // ALL POSTS
        //[GET]: List of post (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Blog(string searchTerm)
        {
            var postResponse = await _supabase.From<Post>().Get();
            var posts = postResponse.Models;

            //Filter by name or description
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                posts = posts.Where(p =>
                    (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchTerm)) ||
                    (!string.IsNullOrEmpty(p.Content) && p.Content.ToLower().Contains(searchTerm))
                ).ToList();
            }

            var farmersResponse = await _supabase.From<User>().Get();

            var viewModel = new BlogViewModel
            {
                Posts = posts,
                Farmers = farmersResponse.Models,
                SearchQuery = searchTerm 
            };

            return View(viewModel);
        }

        //[GET] : Post details from blog (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> DetailPost(int id)
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

            return View("DetailPost", viewModel);
        }







        // PRODUCTS FARMER
        //[GET] : Products from farmer (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Product(string searchTerm)
        {

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index", "Login");

            var farmerQuery = await _supabase.From<User>().Where(x => x.Email == email).Get();
            var farmer = farmerQuery.Models.FirstOrDefault();

            if (farmer == null)
                return NotFound("Farmer not found.");

            var productsResponse = await _supabase
                .From<Product>()
                .Where(p => p.FarmerId == farmer.UserId)
                .Get();

            var allProducts = productsResponse.Models;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                allProducts = allProducts
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Description) && p.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }

            var productTypes = await _supabase.From<ProductType>().Get();

            var viewModel = new FarmerProductViewModel
            {
                Products = allProducts,
                ProductTypes = productTypes.Models,
                SearchTerm = searchTerm
            };

            return View("Product/Index", viewModel);
        }


        //[GET] : Product/Add (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Add1()
        {
            var productTypesResponse = await _supabase.From<ProductType>().Get();

            var viewModel = new AddProductViewModel
            {
                ProductTypes = productTypesResponse.Models
            };

            return View("Product/Add", viewModel);
        }


        //[POST] : Product/Add (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> Add1(AddProductViewModel model)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            Console.WriteLine($"Logged-in user email: {email}");

            if (string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Login required to add a product.";
                return RedirectToAction("Index", "Login");
            }


            var farmerQuery = await _supabase.From<User>().Where(x => x.Email == email).Get();
            var farmer = farmerQuery.Models.FirstOrDefault();

            Console.WriteLine(farmer != null
                ? $"Farmer found: {farmer.Name} ({farmer.UserId})"
                : "Farmer not found in database.");

            if (farmer == null)
            {
                TempData["Error"] = "Farmer account not found.";
                return RedirectToAction("Product");
            }

            var newProduct = model.Product;
            newProduct.DateListed = DateTime.UtcNow;
            newProduct.Status = model.Product.Status;
            newProduct.FarmerId = farmer.UserId;
            model.Product.ImageUrl = "default";


            Console.WriteLine("Product details:");
            Console.WriteLine($"Name: {newProduct.Name}");
            Console.WriteLine($"Description: {newProduct.Description}");
            Console.WriteLine($"Price: {newProduct.Price}");
            Console.WriteLine($"ProductTypeId: {newProduct.ProductTypeId}");
            Console.WriteLine($"Status: {newProduct.Status}");
            Console.WriteLine($"FarmerId: {newProduct.FarmerId}");

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
                    return View("Product/Add", model);
                }

                var response = await _supabase.From<Product>().Insert(newProduct);
                Console.WriteLine("Product inserted into database.");

                TempData["Success"] = "Product added successfully!";
                return RedirectToAction("Product");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                TempData["Error"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("Product");
            }
        }



        //[GET]: Product/Edit (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            var productQuery = await _supabase.From<Product>().Where(x => x.ProductId == id).Get();
            var product = productQuery.Models.FirstOrDefault();

            if (product == null)
            {
                return NotFound("Product not found.");
            }


            var productTypesResponse = await _supabase.From<ProductType>().Get();

            var viewModel = new AddProductViewModel
            {
                Product = product,
                ProductTypes = productTypesResponse.Models
            };

            viewModel.Product.ImageUrl = product.ImageUrl;
            return View("Product/Edit", viewModel);
        }


        //[POST]: Product/Edit (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> Edit(AddProductViewModel model)
        {

            if (!ModelState.IsValid)
            {
                foreach (var entry in ModelState)
                {
                    var key = entry.Key;
                    foreach (var error in entry.Value.Errors)
                    {
                        Console.WriteLine($"[Model Error] Field: {key}, Error: {error.ErrorMessage}");
                    }
                }

                var productTypesResponse = await _supabase.From<ProductType>().Get();
                model.ProductTypes = productTypesResponse.Models;
                return View("Product/Edit", model);
            }

            var existingProductQuery = await _supabase.From<Product>().Where(x => x.ProductId == model.Product.ProductId).Get();
            var existingProduct = existingProductQuery.Models.FirstOrDefault();

            if (existingProduct == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Edit", new { id = model.Product.ProductId });
            }

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login");
            }

            var farmerQuery = await _supabase.From<User>().Where(x => x.Email == email).Get();
            var farmer = farmerQuery.Models.FirstOrDefault();

            if (farmer == null)
            {
                TempData["Error"] = "Farmer not found.";
                return RedirectToAction("Edit", new { id = model.Product.ProductId });
            }


            if (existingProduct.FarmerId != farmer.UserId)
            {
                TempData["Error"] = "You are not authorized to edit this product.";
                return RedirectToAction("Edit", new { id = model.Product.ProductId });
            }

            // Update fields
            existingProduct.Name = model.Product.Name;
            existingProduct.Description = model.Product.Description;
            existingProduct.Category = model.Product.Category;
            existingProduct.ProductTypeId = model.Product.ProductTypeId;
            existingProduct.Amount = model.Product.Amount;
            existingProduct.Price = model.Product.Price;
            existingProduct.Location = model.Product.Location;
            existingProduct.Status = model.Product.Status;

            // Handle file upload for image
            var file = Request.Form.Files["imageFile"];
            if (file != null && file.Length > 0)
            {
                // Generate a unique filename
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
                }

                // Set the new image URL
                existingProduct.ImageUrl = $"https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/{fileName}";
            }
            else
            {
                // Use the same image if no new image is uploaded
                Console.WriteLine("[POST] No new image uploaded. Keeping existing image.");
            }

            // Update product in the database
            var response = await _supabase.From<Product>().Update(existingProduct);

            if (response.Models.Count > 0)
            {
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction("Product", "Farmer");
            }
            else
            {
                TempData["Error"] = "There was an error updating the product.";
                return RedirectToAction("Edit", new { id = model.Product.ProductId });
            }
        }


        //[GET]: Product/Delete (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Get the product by ID
            var productQuery = await _supabase.From<Product>().Where(p => p.ProductId == id).Get();
            var product = productQuery.Models.FirstOrDefault();

            if (product == null)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction("Product");
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

            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction("Product");
        }


        //[GET]: Product/Product Details (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> ProductDetails(int id)
        {
            // 1. Get the product by ID
            var productResponse = await _supabase
                .From<Product>()
                .Filter("product_id", Supabase.Postgrest.Constants.Operator.Equals, id)
                .Get();

            var product = productResponse.Models.FirstOrDefault();

            if (product == null)
            {
                return NotFound(); // Show 404 if product doesn't exist
            }

            // 2. Get the farmer who posted the product
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

            return View("Product/ProductDetails", viewModel);

        }







        // POST FROM FARMER

        //[GET]: Post/List of alll the farmers post (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> Post(string searchTerm)
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return RedirectToAction("Index", "Login");

            var userQuery = await _supabase.From<User>().Where(u => u.Email == email).Get();
            var user = userQuery.Models.FirstOrDefault();

            if (user == null)
                return NotFound("User not found.");

            var postsQuery = _supabase.From<Post>().Where(p => p.UserId == user.UserId);
            var postsResponse = await postsQuery.Get();
            var posts = postsResponse.Models;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                posts = posts
                    .Where(p => (!string.IsNullOrEmpty(p.Title) && p.Title.ToLower().Contains(searchTerm)) ||
                                (!string.IsNullOrEmpty(p.Content) && p.Content.ToLower().Contains(searchTerm)))
                    .ToList();
            }

            var viewModel = new FarmerPostViewModel
            {
                Posts = posts,
                SearchTerm = searchTerm
            };

            return View("Post/Index", viewModel);
        }


        //[GET]: Post/Add (Microsoft, 2025)
        [HttpGet]
        public IActionResult AddPost()
        {
            return View("Post/Add", new AddPostViewModel());
        }

        //[POST]: Post/Add (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> AddPost(AddPostViewModel model)
        {

            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Login");
            }

            var employeeQuery = await _supabase.From<User>().Where(x => x.Email == email).Get();
            var employee = employeeQuery.Models.FirstOrDefault();

            if (employee == null)
            {
                return NotFound("Employee not found.");
            }

            var newPost = model.Post;
            newPost.UserId = employee.UserId;
            newPost.CreatedAt = DateTime.UtcNow;
            newPost.UpdatedAt = DateTime.UtcNow;


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

                newPost.ImageUrl = $"https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/{fileName}";
            }
            else
            {
                newPost.ImageUrl = "https://fpfvdrpoemvkwahfndrd.supabase.co/storage/v1/object/public/agri-photos/default_image.webp";
            }

            try
            {
                var response = await _supabase.From<Post>().Insert(newPost);

                if (!response.ResponseMessage.IsSuccessStatusCode)
                {
                    TempData["Error"] = "There was a problem adding the post.";
                    return View("Post/Add", model);

                }

                TempData["Success"] = "Post added successfully!";
                return RedirectToAction("Post");
            }
            catch (Exception ex)
            {
                return View("Post/Add", model);
            }
        }

        // [GET]: Edit/Post (Microsoft, 2025)
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
            return View("Post/Edit", viewModel);
        }


        // [POST] Edit/Post (Microsoft, 2025)
        [HttpPost]
        public async Task<IActionResult> EditPost(AddPostViewModel model)
        {

            if (!ModelState.IsValid)
            {
                return View("Post/Edit", model);
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

            // === Required Field Validation ===
            if (string.IsNullOrWhiteSpace(existingPost.Title) ||
                string.IsNullOrWhiteSpace(existingPost.Content) ||
                string.IsNullOrWhiteSpace(existingPost.Category))
            {
                TempData["Error"] = "Please fill in all required fields: Title, Content, and Category.";
                return View("Post/Add", model);
            }

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
                return RedirectToAction("Post", "Farmer");
            }

            TempData["Error"] = "Update failed.";
            return RedirectToAction("EditPost", new { id = model.Post.PostId });
        }


        // [GET] Post/Delete (Microsoft, 2025)
        [HttpGet]
        public async Task<IActionResult> DeletePost(int id)
        {
            var postQuery = await _supabase.From<Post>().Where(p => p.PostId == id).Get();
            var post = postQuery.Models.FirstOrDefault();

            if (post == null)
            {
                Console.WriteLine("[DELETE] Post not found.");
                return NotFound("Post not found.");
            }

            // Extract the filename from the ImageUrl
            var imageUrl = post.ImageUrl;
            var storagePath = imageUrl?.Split("/agri-photos/").Last();

            // Only delete image if it's not the default one
            if (!string.IsNullOrEmpty(storagePath) && !imageUrl.Contains("default_image.webp"))
            {
                Console.WriteLine($"[DELETE] Deleting image from storage: {storagePath}");
                await _supabase.Storage
                    .From("agri-photos")
                    .Remove(new List<string> { storagePath });
            }
            else
            {
                Console.WriteLine("[DELETE] No custom image to delete from storage.");
            }

            // Delete the post
            await _supabase
                .From<Post>()
                .Where(p => p.PostId == id)
                .Delete();

            TempData["Success"] = "Post deleted.";
            return RedirectToAction("Post");
        }


        // [POST] Post/Details (Microsoft, 2025)
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

            return View("Post/PostDetails", viewModel); 
        }

    }
}
