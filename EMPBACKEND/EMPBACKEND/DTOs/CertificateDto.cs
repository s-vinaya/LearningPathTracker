using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class CertificateDto
    {
        public int CertificateId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int LearningPathId { get; set; }
        public string PathTitle { get; set; } = string.Empty;
        public string CertificateNumber { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? CertificateUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateCertificateDto
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int LearningPathId { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
    }

    public class UpdateCertificateDto
    {
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
    }
}