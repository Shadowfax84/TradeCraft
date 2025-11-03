using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class StockRecord
    {
        [Key]
        public Guid RecordId { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(20)]
        [ForeignKey(nameof(CompanyProfile))]
        public string TickerSymbol { get; set; } = null!;

        public DateTime Date { get; set; }

        public decimal? Open { get; set; }

        public decimal? High { get; set; }

        public decimal? Low { get; set; }

        public decimal? Close { get; set; }

        public decimal? AdjustedClose { get; set; }

        public long? Volume { get; set; }

    }
}