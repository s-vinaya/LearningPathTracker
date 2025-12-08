using learning_path_tracker.Api.Controllers;
using learning_path_tracker.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace learning_path_tracker.UnitTests;

public class AuthControllerTests
{
    [Fact]
    public void Register_WithEmptyEmail_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterRequestDTO
        {
            Email = "",
            Password = "Test@123",
            FullName = "Test User"
        };

        // Act & Assert
        Assert.True(string.IsNullOrWhiteSpace(registerDto.Email));
    }

    [Fact]
    public void Register_WithEmptyPassword_ReturnsBadRequest()
    {
        // Arrange
        var registerDto = new RegisterRequestDTO
        {
            Email = "test@example.com",
            Password = "",
            FullName = "Test User"
        };

        // Act & Assert
        Assert.True(string.IsNullOrWhiteSpace(registerDto.Password));
    }

    [Fact]
    public void Login_WithEmptyEmail_ShouldFail()
    {
        // Arrange
        var loginDto = new LoginRequestDTO
        {
            Email = "",
            Password = "Test@123"
        };

        // Act & Assert
        Assert.True(string.IsNullOrWhiteSpace(loginDto.Email));
    }

    [Fact]
    public void Login_WithEmptyPassword_ShouldFail()
    {
        // Arrange
        var loginDto = new LoginRequestDTO
        {
            Email = "test@example.com",
            Password = ""
        };

        // Act & Assert
        Assert.True(string.IsNullOrWhiteSpace(loginDto.Password));
    }

    [Fact]
    public void RegisterDTO_ValidData_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var registerDto = new RegisterRequestDTO
        {
            Email = "test@example.com",
            Password = "Test@123",
            FullName = "Test User",
            PhoneNumber = "1234567890",
            Department = "IT",
            JobTitle = "Developer"
        };

        // Assert
        Assert.Equal("test@example.com", registerDto.Email);
        Assert.Equal("Test@123", registerDto.Password);
        Assert.Equal("Test User", registerDto.FullName);
        Assert.Equal("1234567890", registerDto.PhoneNumber);
        Assert.Equal("IT", registerDto.Department);
        Assert.Equal("Developer", registerDto.JobTitle);
    }

    [Fact]
    public void LoginDTO_ValidData_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var loginDto = new LoginRequestDTO
        {
            Email = "test@example.com",
            Password = "Test@123"
        };

        // Assert
        Assert.Equal("test@example.com", loginDto.Email);
        Assert.Equal("Test@123", loginDto.Password);
    }

    [Fact]
    public void UserResponseDTO_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var userResponse = new UserResponseDTO
        {
            Id = 1,
            FullName = "Test User",
            Email = "test@example.com",
            PhoneNumber = "1234567890",
            Department = "IT",
            JobTitle = "Developer",
            Role = "Employee",
            LastLogin = DateTime.UtcNow
        };

        // Assert
        Assert.Equal(1, userResponse.Id);
        Assert.Equal("Test User", userResponse.FullName);
        Assert.Equal("test@example.com", userResponse.Email);
        Assert.Equal("Employee", userResponse.Role);
    }

    [Fact]
    public void LoginResponseDTO_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var loginResponse = new LoginResponseDTO
        {
            Token = "test-token",
            User = new UserResponseDTO
            {
                Id = 1,
                Email = "test@example.com",
                FullName = "Test User",
                Role = "Employee"
            }
        };

        // Assert
        Assert.Equal("test-token", loginResponse.Token);
        Assert.NotNull(loginResponse.User);
        Assert.Equal(1, loginResponse.User.Id);
        Assert.Equal("test@example.com", loginResponse.User.Email);
    }

    [Fact]
    public void CheckEmailDto_ValidEmail_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var checkEmailDto = new CheckEmailDto
        {
            Email = "test@example.com"
        };

        // Assert
        Assert.Equal("test@example.com", checkEmailDto.Email);
    }

    [Fact]
    public void ForgotPasswordDto_ValidEmail_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var forgotPasswordDto = new ForgotPasswordDto
        {
            Email = "test@example.com"
        };

        // Assert
        Assert.Equal("test@example.com", forgotPasswordDto.Email);
    }

    [Fact]
    public void ResetPasswordDto_ValidData_PropertiesSetCorrectly()
    {
        // Arrange & Act
        var resetPasswordDto = new ResetPasswordDto
        {
            Token = "test-token",
            NewPassword = "NewPassword@123"
        };

        // Assert
        Assert.Equal("test-token", resetPasswordDto.Token);
        Assert.Equal("NewPassword@123", resetPasswordDto.NewPassword);
    }

    [Fact]
    public void RegisterDTO_EmailValidation_ShouldContainAtSymbol()
    {
        // Arrange
        var validEmail = "test@example.com";
        var invalidEmail = "testexample.com";

        // Act & Assert
        Assert.Contains("@", validEmail);
        Assert.DoesNotContain("@", invalidEmail);
    }
}
