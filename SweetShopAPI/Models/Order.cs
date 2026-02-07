using System.ComponentModel.DataAnnotations.Schema;

namespace SweetShopAPI.Models
{
    [Table("Orders")]
    public class Order
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("customerName")]
        public string CustomerName { get; set; } = string.Empty;

        [Column("customerPhone")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Column("customerAddress")]
        public string CustomerAddress { get; set; } = string.Empty;

        [Column("totalAmount")]
        public decimal TotalAmount { get; set; }

        [Column("orderDate")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
