using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TC_Backend.Models
{
    public class User : IdentityUser
    {
        [Required]
        public int RoleID { get; set; }

        public DateTime LastLogin { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(2083)]
        public string? ProfilePictureUrl { get; set; }
        public string? ProfilePicturePublicId { get; set; }
    }
}