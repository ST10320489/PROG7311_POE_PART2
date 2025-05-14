using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Agri_Energy.Models
{
    [Table("products")]  // Supabase table name for products
    public class Product : BaseModel
    {
        [PrimaryKey("product_id", false)]  // Primary key for the products table
        public int ProductId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Product name is required")]
        public string Name { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("category")]
        public string Category { get; set; }

        [Column("product_type_id")]
        [Required(ErrorMessage = "Product Type is required")]
        public int ProductTypeId { get; set; }

        [Column("amount")]
        [Range(1, int.MaxValue, ErrorMessage = "Amount must be a number greater than 0")]
        public int Amount { get; set; }

        [Column("price")]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a valid number")]
        public double Price { get; set; }

        [Column("date_listed")]
        public DateTime DateListed { get; set; }

        [Column("status")]
        [RegularExpression("^(Available|Sold)$", ErrorMessage = "Status must be 'Available' or 'Sold'")]
        public string Status { get; set; } // "Available", "Sold", etc.

        [Column("location")]
        public string Location { get; set; }

        [Column("farmer_id")]
        public int FarmerId { get; set; }

        [Column("image_url")]
        [Display(Name = "Image")]
        public string ImageUrl { get; set; }  // or PhotoUrl

    }
}


