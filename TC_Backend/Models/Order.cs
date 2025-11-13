using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class Order
    {
        [Key]
        public Guid OrderID { get; set; } = Guid.NewGuid();

        [Required]
        public OrderType OrderType { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserID { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [ForeignKey(nameof(CompanyList))]
        public string TickerSymbol { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public OrderStatus OrderStatus { get; set; }

        public DateTime CreatedTimestamp { get; set; } = DateTime.UtcNow;

        public DateTime? ExecutedTimestamp { get; set; }

        public int? RemainingQuantity { get; set; }
    }

    public enum OrderType
    {
        Buy,
        Sell
    }

    public enum OrderStatus
    {
        Pending,
        Executed,
        Cancelled
    }
}