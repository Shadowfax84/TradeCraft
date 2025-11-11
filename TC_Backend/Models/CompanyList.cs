using System.ComponentModel.DataAnnotations;

namespace TC_Backend.Models
{
    public class CompanyList
    {
        [Key]
        public Guid ClId { get; set; }
        [Required]
        public required string CompanyName { get; set; }
        [Required]
        [MaxLength(20)]
        public required string TickerSymbol { get; set; }
    }
}