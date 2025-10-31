using System.ComponentModel.DataAnnotations;

namespace EMPBACKEND.DTOs
{
    public class UserAssessmentDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int AssessmentId { get; set; }
        public string AssessmentTitle { get; set; } = string.Empty;
        public int Score { get; set; }
        public DateTime AttemptDate { get; set; }
        public bool IsPassed { get; set; }
    }

    public class CreateUserAssessmentDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int AssessmentId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Score { get; set; }

        [Required]
        public bool IsPassed { get; set; }
    }

    public class UpdateUserAssessmentDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int AssessmentId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Score { get; set; }

        [Required]
        public bool IsPassed { get; set; }
    }
}