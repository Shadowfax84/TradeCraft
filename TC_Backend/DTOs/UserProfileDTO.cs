using System.ComponentModel.DataAnnotations;

namespace TC_Backend.DTOs
{
    public class UserProfileDTO
    {
        public string UserName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? RoleName { get; set; }

        public DateTime? LastLogin { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}