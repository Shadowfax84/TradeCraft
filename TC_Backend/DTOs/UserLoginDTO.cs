using System.ComponentModel.DataAnnotations;

namespace TC_Backend.DTOs
{
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
    }
}
