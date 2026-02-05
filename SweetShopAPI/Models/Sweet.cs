using System.ComponentModel.DataAnnotations.Schema;
namespace SweetShopAPI.Models
{
    [Table("Sweets")]
    public class Sweet
    {
        [Column("id")] // Maps C# "Id" to Postgres "id"
        public int Id { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("imageUrl")] // Note: This matches the "imageUrl" we created in SQL
        public string? ImageUrl { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("tag")]
        public string? Tag { get; set; }
    }
}
