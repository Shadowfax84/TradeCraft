using System.ComponentModel.DataAnnotations;

namespace TC_Backend.Models
{
    public class GameSubModule
    {
        [Key]
        public Guid SubModuleID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid ModuleID { get; set; } 

        [Required]
        [MaxLength(100)]
        public string SubModuleName { get; set; } = null!;
        [Range(0,9999)]
        public int Points { get; set; }  // Points awarded upon completion

        public int OrderIndex { get; set; }  // Sequence/order inside parent module

        public bool IsSkippable { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}