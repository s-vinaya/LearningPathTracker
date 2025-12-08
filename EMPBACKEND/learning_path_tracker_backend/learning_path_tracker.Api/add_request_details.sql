-- Add RequestDetails column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Approvals]') AND name = 'RequestDetails')
BEGIN
    ALTER TABLE [dbo].[Approvals] ADD [RequestDetails] nvarchar(max) NULL;
    PRINT 'RequestDetails column added successfully';
END
ELSE
BEGIN
    PRINT 'RequestDetails column already exists';
END
GO
