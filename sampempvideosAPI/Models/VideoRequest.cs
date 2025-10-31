using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMPBACKEND.Models
{
    public class VideoRequest
    {
        public int Id { get; set; }
        
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string VideoTitle { get; set; } = string.Empty;
        
        [StringLength(1000)]
        public string RequestDescription { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        [RegularExpression("^(Pending|Approved|Rejected)$")]
        public string Status { get; set; } = "Pending";
        
        [Required]
        public DateTime RequestedDate { get; set; } = DateTime.UtcNow;
    }
}