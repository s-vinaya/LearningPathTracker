using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMPBACKEND.Models
{
    public class Video
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [Url]
        [StringLength(500)]
        public string VideoUrl { get; set; } = string.Empty;
        
        [Required]
        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        [Required]
        [Range(1, 86400)]
        public int Duration { get; set; } // in seconds
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();
    }
}