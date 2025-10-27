using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Video
    {
        public int Id { get; set; }
        
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        public string VideoUrl { get; set; } = string.Empty;
        
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        
        public int Duration { get; set; } // in seconds
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public ICollection<VideoProgress> VideoProgresses { get; set; } = new List<VideoProgress>();
    }
}