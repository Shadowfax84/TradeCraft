using System.ComponentModel.DataAnnotations;

namespace TC_Backend.Models
{
    public class GameModule
    {
        [Key]
        public Guid ModuleID { get; set; } = Guid.NewGuid();

        [Required]
        [MaxLength(100)]
        public string ModuleName { get; set; } = null!;

        [Required]
        public int RoleID { get; set; }
        [Range(0,9999)]
        public int? RequiredPoints { get; set; }  // Points required to unlock this module, nullable

        public int OrderIndex { get; set; }  // Defines display or progression sequence in a role

        public bool IsActive { get; set; } = true;  // Indicates if this module is currently active

    }
}