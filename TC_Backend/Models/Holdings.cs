using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class Holdings
    {
        [Key]
        public Guid PortfolioId { get; set; } = Guid.NewGuid();

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!;

        [Required]
        [MaxLength(20)]
        [ForeignKey(nameof(CompanyList))]
        public string TickerSymbol { get; set; } = null!;

        [Required]
        public int QuantityOwned { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PurchasePrice { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    }
}