using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.Models
{
    public class OtpCode
    {
        public int Id { get; set; }
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(6)]
        public string Code { get; set; } = string.Empty;
        
        public DateTime ExpiryTime { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}