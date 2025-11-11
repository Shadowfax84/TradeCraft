using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class CompanyProfile
    {
        [Key]
        public Guid CPId { get; set; }

        [Required]
        [MaxLength(20)]
        [ForeignKey(nameof(CompanyList))]
        public string TickerSymbol { get; set; } = null!;

        public string? Address { get; set; }

        public string? Phone { get; set; }

        public string? Website { get; set; }

        public string? Sector { get; set; }

        public string? Industry { get; set; }

        public long? CntEmployees { get; set; }

        public string? Description { get; set; }
    }
}