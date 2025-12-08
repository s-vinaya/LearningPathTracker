using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace learning_path_tracker.Api.Services;

public class CertificateGeneratorService
{
    public byte[] GenerateCertificatePdf(string certificateId, string employeeName, string courseName, 
        string learningPathName, string managerName, double averageScore, DateTime issuedAt, string certificateType)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(0);
                
                page.Content().Background("#1a1a2e").Column(column =>
                {
                    // Logo at top left
                    column.Item().PaddingLeft(30).PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem().AlignLeft().Column(logoCol =>
                        {
                            var logoPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "logo.png");
                            if (File.Exists(logoPath))
                            {
                                logoCol.Item().Height(50).Image(logoPath);
                            }
                            else
                            {
                                logoCol.Item().Text("🎓 Learn Track").FontSize(18).Bold().FontColor("#00d4ff");
                            }
                        });
                        row.RelativeItem();
                    });
                    
                    column.Item().PaddingTop(40).AlignCenter().Text("CERTIFICATE OF COMPLETION")
                        .FontSize(36).Bold().FontColor("#ffd700");
                    
                    column.Item().PaddingTop(20).AlignCenter().Text("This is to certify that")
                        .FontSize(16).FontColor("#ffffff");
                    
                    column.Item().PaddingTop(10).AlignCenter().Text(employeeName)
                        .FontSize(32).Bold().FontColor("#00d4ff");
                    
                    column.Item().PaddingTop(15).AlignCenter().Text("has successfully completed")
                        .FontSize(16).FontColor("#ffffff");
                    
                    column.Item().PaddingTop(10).AlignCenter().Text(certificateType == "LearningPath" ? learningPathName : courseName)
                        .FontSize(24).Bold().FontColor("#ffd700");
                    
                    column.Item().PaddingTop(20).AlignCenter().Text($"Score: {averageScore:F1}%")
                        .FontSize(20).FontColor("#00ff88");
                    
                    column.Item().PaddingTop(40).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Text(issuedAt.ToString("MMMM dd, yyyy"))
                                .FontSize(14).FontColor("#ffffff");
                            col.Item().PaddingTop(5).AlignCenter().Text("Date")
                                .FontSize(12).FontColor("#888888");
                        });
                        
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().AlignCenter().Text(managerName ?? "System")
                                .FontSize(14).FontColor("#ffffff");
                            col.Item().PaddingTop(5).AlignCenter().Text("Authorized By")
                                .FontSize(12).FontColor("#888888");
                        });
                    });
                    
                    column.Item().PaddingTop(20).AlignCenter().Text($"Certificate ID: {certificateId}")
                        .FontSize(10).FontColor("#666666");
                });
            });
        });
        
        return document.GeneratePdf();
    }
}
