using Supabase.Postgrest.Models;
using Supabase.Postgrest.Attributes;
using System;
using System.Collections.Generic;

namespace Agri_Energy.Models
{
    [Table("product_types")]  // Supabase table name for product types
    public class ProductType : BaseModel
    {
        [PrimaryKey("product_type_id", false)]  // Primary key for the product_types table
        public int ProductTypeId { get; set; }

        [Column("type_name")]
        public string TypeName { get; set; }

    }
}
