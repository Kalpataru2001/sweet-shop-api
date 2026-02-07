using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SweetShopAPI.Models
{

    [Table("OrderItems")]
    public class OrderItem
    {
        // Database column is "Id" (Capital 'I')
        [Column("Id")]
        public int Id { get; set; }

        // Database column is "Quantity" (Capital 'Q')
        [Column("Quantity")]
        public decimal Quantity { get; set; }

        // Database column is "Price" (Capital 'P')
        [Column("Price")]
        public decimal Price { get; set; }

        // Database column is "OrderId" (Capital 'O')
        [Column("OrderId")]
        public int OrderId { get; set; }

        [JsonIgnore]
        public Order? Order { get; set; }

        // Database column is "SweetId" (Capital 'S')
        [Column("SweetId")]
        public int SweetId { get; set; }

        public Sweet? Sweet { get; set; }
    }
}

