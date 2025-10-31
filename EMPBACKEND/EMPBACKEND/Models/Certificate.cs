using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class Certificate
    {
        public int CertificateId { get; set; }
        
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        
        public int PathId { get; set; }
        public LearningPath LearningPath { get; set; } = null!;
        
        [StringLength(100)]
        public string CertificateNumber { get; set; } = string.Empty;
        
        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ExpiresAt { get; set; }
        
        public string? CertificateUrl { get; set; } // PDF file path/URL
        public bool IsActive { get; set; } = true;
    }
}