using System.ComponentModel.DataAnnotations;

namespace TC_Backend.DTOs
{
    public class CompanyListDTO
    {
        [Required]
        public required string CompanyName { get; set; }
        [Required]
        public required string TickerSymbol { get; set; }
    }
}