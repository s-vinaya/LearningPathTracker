namespace learning_path_tracker.Application.DTOs.Dashboard;

public class DashboardSummaryDto
{
    public int TotalUsers { get; set; }
    public int ActiveCourses { get; set; }
    public int CoursesCompleted { get; set; }
    public int CertificatesIssued { get; set; }
    public double EmailDeliveryRate { get; set; }
}
