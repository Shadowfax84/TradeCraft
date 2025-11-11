using System.ComponentModel.DataAnnotations;

namespace TC_Backend.DTOs
{
    public class UserSignupDTO
    {
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        public IFormFile? ProfilePicture { get; set; }
    }
}
