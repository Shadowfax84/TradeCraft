using System.ComponentModel.DataAnnotations;

namespace TC_Backend.Models
{
    public class Role
    {
        [Key]
        public int RoleId { get; set; }
        [Required]
        [MaxLength(20)]
        private string _roleName = null!;

        public string RoleName
        {
            get => _roleName;
            set => _roleName = (value ?? "Investor").ToLower();

        } 
        [Range(0, 9999)]
        public int RequiredPoints { get; set; }
        [Range(1,3)]
        public int RoleLevel { get; set; }
    }
}