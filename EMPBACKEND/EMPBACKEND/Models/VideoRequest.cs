using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class VideoRequest
    {
        public int Id { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [StringLength(200)]
        public string VideoTitle { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string RequestDescription { get; set; } = string.Empty;
        
        [StringLength(50)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
        
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    }
}