using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agri_Energy.Models
{
    [Table("posts")]
    public class Post : BaseModel
    {
        [PrimaryKey("post_id", false)]
        public int PostId { get; set; }

        [Column("user_id")]
        public int UserId { get; set; }

        [Column("title")]
        [Required(ErrorMessage = "Title is required.")]
        public string Title { get; set; }

        [Column("content")]
        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [Column("image_url")]
        public string ImageUrl { get; set; }

        [Column("category")]
        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}
