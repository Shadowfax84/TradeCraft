using System.ComponentModel.DataAnnotations;

namespace TC_Backend.DTOs
{
    public class RoleDTO
    {
        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; } = null!;

        [Range(0, 9999)]
        public int RequiredPoints { get; set; }

        [Range(1, 3)]
        public int RoleLevel { get; set; }
    }
}
