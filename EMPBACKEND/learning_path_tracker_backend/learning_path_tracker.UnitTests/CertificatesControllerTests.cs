using learning_path_tracker.Api.Controllers;
using learning_path_tracker.Application.DTOs;
using learning_path_tracker.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace learning_path_tracker.UnitTests;

public class CertificatesControllerTests
{
    [Fact]
    public async Task VerifyCertificate_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "CERT-12345";
        var expectedCertificate = new CertificateDto
        {
            Id = 1,
            CertificateId = certificateId,
            EmployeeName = "John Doe",
            CourseName = "C# Programming",
            LearningPathName = "Backend Development",
            ManagerName = "Jane Smith",
            AverageScore = 95.5,
            IssuedAt = DateTime.UtcNow,
            CertificateType = "Course Completion"
        };
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync(expectedCertificate);
        var controller = new CertificatesController(mockService.Object);

        // Act
        var result = await controller.VerifyCertificate(certificateId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCertificate = Assert.IsType<CertificateDto>(okResult.Value);
        Assert.Equal(certificateId, returnedCertificate.CertificateId);
        Assert.Equal("John Doe", returnedCertificate.EmployeeName);
    }

    [Fact]
    public async Task VerifyCertificate_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "INVALID-ID";
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync((CertificateDto?)null);
        var controller = new CertificatesController(mockService.Object);

        // Act
        var result = await controller.VerifyCertificate(certificateId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task VerifyCertificate_WithEmptyId_ReturnsNotFound()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "";
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync((CertificateDto?)null);
        var controller = new CertificatesController(mockService.Object);

        // Act
        var result = await controller.VerifyCertificate(certificateId);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task VerifyCertificate_ServiceCallsOnce_VerifiesServiceInvocation()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "CERT-12345";
        var certificate = new CertificateDto
        {
            Id = 1,
            CertificateId = certificateId,
            EmployeeName = "Test User"
        };
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync(certificate);
        var controller = new CertificatesController(mockService.Object);

        // Act
        await controller.VerifyCertificate(certificateId);

        // Assert
        mockService.Verify(s => s.VerifyCertificateAsync(certificateId), Times.Once);
    }

    [Fact]
    public async Task VerifyCertificate_WithValidCertificate_ReturnsCorrectData()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "CERT-99999";
        var issuedDate = new DateTime(2024, 1, 15);
        var expectedCertificate = new CertificateDto
        {
            Id = 5,
            CertificateId = certificateId,
            EmployeeName = "Alice Johnson",
            CourseName = "Advanced JavaScript",
            LearningPathName = "Frontend Development",
            ManagerName = "Bob Williams",
            AverageScore = 88.0,
            IssuedAt = issuedDate,
            CertificateType = "Learning Path Completion"
        };
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync(expectedCertificate);
        var controller = new CertificatesController(mockService.Object);

        // Act
        var result = await controller.VerifyCertificate(certificateId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var certificate = Assert.IsType<CertificateDto>(okResult.Value);
        Assert.Equal(5, certificate.Id);
        Assert.Equal("Alice Johnson", certificate.EmployeeName);
        Assert.Equal("Advanced JavaScript", certificate.CourseName);
        Assert.Equal(88.0, certificate.AverageScore);
        Assert.Equal(issuedDate, certificate.IssuedAt);
    }

    [Fact]
    public void CertificateDto_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var certificate = new CertificateDto
        {
            Id = 1,
            CertificateId = "CERT-123",
            EmployeeName = "John Doe",
            CourseName = "Test Course",
            LearningPathName = "Test Path",
            ManagerName = "Manager Name",
            AverageScore = 95.5,
            IssuedAt = DateTime.UtcNow,
            CertificateType = "Course"
        };

        // Assert
        Assert.Equal(1, certificate.Id);
        Assert.Equal("CERT-123", certificate.CertificateId);
        Assert.Equal("John Doe", certificate.EmployeeName);
        Assert.Equal(95.5, certificate.AverageScore);
    }

    [Fact]
    public async Task VerifyCertificate_WithHighScore_ReturnsCorrectScore()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "CERT-HIGH-SCORE";
        var certificate = new CertificateDto
        {
            Id = 15,
            CertificateId = certificateId,
            EmployeeName = "Top Performer",
            AverageScore = 99.9
        };
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync(certificate);
        var controller = new CertificatesController(mockService.Object);

        // Act
        var result = await controller.VerifyCertificate(certificateId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCert = Assert.IsType<CertificateDto>(okResult.Value);
        Assert.Equal(99.9, returnedCert.AverageScore);
    }

    [Fact]
    public async Task VerifyCertificate_WithDifferentCertificateTypes_ReturnsCorrectType()
    {
        // Arrange
        var mockService = new Mock<ICertificateService>();
        var certificateId = "CERT-TYPE-TEST";
        var certificate = new CertificateDto
        {
            Id = 10,
            CertificateId = certificateId,
            EmployeeName = "Test Employee",
            CertificateType = "Quiz Completion"
        };
        mockService.Setup(s => s.VerifyCertificateAsync(certificateId))
            .ReturnsAsync(certificate);
        var controller = new CertificatesController(mockService.Object);

        // Act
        var result = await controller.VerifyCertificate(certificateId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedCert = Assert.IsType<CertificateDto>(okResult.Value);
        Assert.Equal("Quiz Completion", returnedCert.CertificateType);
    }
}
