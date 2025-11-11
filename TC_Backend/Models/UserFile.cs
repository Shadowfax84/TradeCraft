using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TC_Backend.Models
{
    public class UserFile
    {
        [Key]
        public Guid FileId { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        public string UserId { get; set; } = null!; 

        [Required]
        [MaxLength(2083)]
        public string Url { get; set; } = null!;

        [Required]
        public string PublicId { get; set; } = null!; 

        public DateTime UploadedOn { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? FileName { get; set; }
        
        [MaxLength(50)]
        public string? FileType { get; set; }
    }
}