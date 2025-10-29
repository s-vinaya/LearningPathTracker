using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class OtpCodeDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; }
    }

    public class CreateOtpCodeDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Code { get; set; } = string.Empty;
        
        [Required]
        public DateTime ExpiryTime { get; set; }
    }

    public class UpdateOtpCodeDto
    {
        [Required]
        public bool IsUsed { get; set; }
    }
}