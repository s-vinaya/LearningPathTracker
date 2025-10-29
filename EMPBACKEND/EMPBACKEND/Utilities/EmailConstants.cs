namespace EMPBACKEND.Utilities
{
    public static class EmailConstants
    {
        public static class Subjects
        {
            public const string OTP = "Password Reset OTP - Employee Learning Portal";
            public const string PasswordResetConfirmation = "Password Reset Successful - Employee Learning Portal";
            public const string CourseAssignment = "New Course Assignment - Employee Learning Portal";
            public const string Welcome = "Registration Successful - Pending Admin Approval";
            public const string Approval = "Account Approved - Welcome to Employee Learning Portal!";
            public const string Rejection = "Account Registration - Update Required";
        }

        public static class Bodies
        {
            public const string OTP = @"
                <h2>Password Reset Request</h2>
                <p>Your OTP code for password reset is: <strong>{0}</strong></p>
                <p>This code will expire in 10 minutes.</p>
                <p>If you didn't request this, please ignore this email.</p>";

            public const string PasswordResetConfirmation = @"
                <h2>Password Reset Successful</h2>
                <p>Your password has been successfully reset.</p>
                <p>If you didn't make this change, please contact your administrator immediately.</p>";

            public const string CourseAssignment = @"
                <h2>New Course Assignment</h2>
                <p>You have been assigned to the course: <strong>{0}</strong></p>
                <p>Please log in to the Employee Learning Portal to start your learning journey.</p>";

            public const string Welcome = @"
                <h2>Registration Successful!</h2>
                <p>Dear {0} {1},</p>
                <p>Your account has been successfully created in the Employee Learning Portal.</p>
                <p><strong>Important:</strong> Your account is currently pending admin approval. You will receive another email once your account is approved and you can start logging in.</p>
                <p>Thank you for your patience.</p>
                <p>Best regards,<br/>Employee Learning Portal Team</p>";

            public const string Approval = @"
                <h2>Account Approved!</h2>
                <p>Dear {0} {1},</p>
                <p>Great news! Your account has been approved by the administrator.</p>
                <p>You can now log in to the Employee Learning Portal and start exploring our courses and learning materials.</p>
                <p>If you have any questions, please don't hesitate to contact our support team.</p>
                <p>Happy Learning!</p>
                <p>Best regards,<br/>Employee Learning Portal Team</p>";

            public const string Rejection = @"
                <h2>Account Registration Update</h2>
                <p>Dear {0} {1},</p>
                <p>We regret to inform you that your account registration could not be approved at this time.</p>
                <p>This may be due to incomplete information or other administrative requirements.</p>
                <p>Please contact your HR department or system administrator for more information on how to proceed.</p>
                <p>Thank you for your understanding.</p>
                <p>Best regards,<br/>Employee Learning Portal Team</p>";
        }
    }
}