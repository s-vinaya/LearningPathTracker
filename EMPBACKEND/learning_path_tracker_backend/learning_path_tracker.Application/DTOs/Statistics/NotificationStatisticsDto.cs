namespace learning_path_tracker.Application.DTOs.Statistics;

public class NotificationStatisticsDto
{
    public int TotalSent { get; set; }
    public string DeliveryRate { get; set; } = string.Empty;
}
