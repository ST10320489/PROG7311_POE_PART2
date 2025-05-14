using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;

namespace Agri_Energy.Models
{
    [Table("users")]
    public class User : BaseModel
    {
        [PrimaryKey("user_id", false)]
        public int UserId { get; set; }

        [Column("name")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Column("surname")]
        [Required(ErrorMessage = "Surname is required.")]
        public string Surname { get; set; }

        [Column("email")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Column("password")]
        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-=\[\]{};':\\|,.<>\/?]).{8,}$",
            ErrorMessage = "Password must contain at least 1 uppercase letter, 1 number, and 1 special character.")]
        public string Password { get; set; }


        [Column("role")]
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; }

        [Column("location")]
        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

    }
}
