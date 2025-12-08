namespace learning_path_tracker.Application.Constants;

public static class EmailConstants
{
    public const string RegistrationSubject = "Welcome to Learning Path Tracker - Registration Successful";
    public const string ApprovalSubject = "Account Approved - Welcome to Learning Path Tracker";
    public const string PasswordResetSubject = "Password Reset Request";
    public const string UserApprovedMessage = "User approved successfully";
    public const string NotificationSentMessage = "Notification sent successfully to all users";
    public const string UserAlreadyExistsMessage = "User already exists";
    public const string InvalidCredentialsMessage = "Invalid credentials";
    public const string UserNotFoundMessage = "User not found";
    public const string NotificationCreatedMessage = "Notification created and sent successfully";
    public const string LearningReminderSubject = "Time to Continue Your Learning Journey!";
    
    public static string GetLearningReminderBody(string userName) => $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #7c3aed;'>⏰ It's Learning Time!</h2>
            <p>Hi {userName},</p>
            <p>This is your scheduled reminder to continue your learning journey.</p>
            <p>You've set aside this time for learning - let's make the most of it!</p>
            <a href='http://localhost:4200/employee/dashboard' style='display: inline-block; padding: 12px 24px; background: #7c3aed; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0;'>Start Learning</a>
            <p>Keep up the great work and stay consistent with your learning goals!</p>
            <br>
            <p>Best regards,<br>Learning Path Tracker Team</p>
        </body>
        </html>";
    
    public static string GetRegistrationBody(string userName) => $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #7c3aed;'>Welcome to Learning Path Tracker!</h2>
            <p>Hi {userName},</p>
            <p>Thank you for registering with us. Your account has been created successfully.</p>
            <p><strong>Please note:</strong> Your account is pending approval from an administrator.</p>
            <p>You will receive another email once your account is approved and you can start learning.</p>
            <br>
            <p>Best regards,<br>Learning Path Tracker Team</p>
        </body>
        </html>";

    public static string GetApprovalBody(string userName) => $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #22c55e;'>Account Approved!</h2>
            <p>Hi {userName},</p>
            <p>Great news! Your account has been approved by our administrator.</p>
            <p>You can now log in and start exploring our courses and learning paths.</p>
            <a href='http://localhost:4200/login' style='display: inline-block; padding: 12px 24px; background: #7c3aed; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0;'>Login Now</a>
            <br>
            <p>Best regards,<br>Learning Path Tracker Team</p>
        </body>
        </html>";

    public static string GetPasswordResetBody(string userName, string resetToken) => $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #7c3aed;'>Password Reset Request</h2>
            <p>Hi {userName},</p>
            <p>We received a request to reset your password.</p>
            <p>Click the button below to reset your password:</p>
            <a href='http://localhost:4200/reset-password?token={resetToken}' style='display: inline-block; padding: 12px 24px; background: #7c3aed; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0;'>Reset Password</a>
            <p>If you didn't request this, please ignore this email.</p>
            <p>This link will expire in 24 hours.</p>
            <br>
            <p>Best regards,<br>Learning Path Tracker Team</p>
        </body>
        </html>";

    public static string GetNotificationBody(string userName, string title, string message) => $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #7c3aed;'>{title}</h2>
            <p>Hi {userName},</p>
            <p>{message}</p>
            <br>
            <p>Best regards,<br>Learning Path Tracker Team</p>
        </body>
        </html>";
}
