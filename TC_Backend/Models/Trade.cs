using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class Trade
    {
        [Key]
        public Guid TradeId { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey(nameof(User))]
        public string BuyerUserId { get; set; } = null!;

        [Required]
        [ForeignKey(nameof(User))]
        public string SellerUserId { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [ForeignKey(nameof(CompanyList))]
        public string TickerSymbol { get; set; } = null!;

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}