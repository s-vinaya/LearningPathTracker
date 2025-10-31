using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Department
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<User> Users { get; set; } = new List<User>();
    }
}