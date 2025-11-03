using System.ComponentModel.DataAnnotations;

namespace TC_Backend.Models
{
    public class UserProgress
    {
        [Key]
        public Guid ProgressID { get; set; } = Guid.NewGuid();

        [Required]
        public Guid UserID { get; set; } 

        [Required]
        public Guid ModuleID { get; set; } 

        [Required]
        public Guid SubmoduleID { get; set; }  

        [Required]
        public bool IsCompleted { get; set; }

        [Required]
        public int PointsEarned { get; set; }

        [Required]
        public DateTime CompletionDate { get; set; }
    }
}