using System.ComponentModel.DataAnnotations;

namespace TC_Backend.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [MaxLength(20)]
        public string RoleName { get; set; } = null!;
        [Range(0, 9999)]
        public int RequiredPoints { get; set; }
        [Range(1,3)]
        public int RoleLevel { get; set; }
    }
}