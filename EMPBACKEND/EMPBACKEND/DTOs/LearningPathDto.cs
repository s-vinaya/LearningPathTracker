namespace EMPBACKEND.DTOs
{
    public class LearningPathDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public List<LearningPathCourseDto> Courses { get; set; } = new List<LearningPathCourseDto>();
    }

    public class CreateLearningPathDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatedBy { get; set; }
        public List<CreateLearningPathCourseDto> Courses { get; set; } = new List<CreateLearningPathCourseDto>();
    }

    public class UpdateLearningPathDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class LearningPathCourseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int Order { get; set; }
        public bool IsRequired { get; set; }
    }

    public class CreateLearningPathCourseDto
    {
        public int CourseId { get; set; }
        public int Order { get; set; }
        public bool IsRequired { get; set; } = true;
    }
}