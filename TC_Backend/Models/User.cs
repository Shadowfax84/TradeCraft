using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class User
    {
        [Key]
        public Guid UserID { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(50)]
        
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [ForeignKey(nameof(Role))]
        public int RoleID { get; set; }

        public DateTime LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(2083)]
        public string? ProfilePictureUrl { get; set; }
    }
}