IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [CompanySubscriptionTypes] (
    [CompanySubscriptionTypeId] int NOT NULL IDENTITY,
    [PackageType] int NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [JobPostLimit] int NOT NULL,
    [CvMatchLimit] int NOT NULL,
    [TrendingJobLimit] int NOT NULL,
    [DurationInDays] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CompanySubscriptionTypes] PRIMARY KEY ([CompanySubscriptionTypeId])
);
GO

CREATE TABLE [Embeddings] (
    [Id] int NOT NULL IDENTITY,
    [Text] nvarchar(max) NOT NULL,
    [Model] nvarchar(max) NOT NULL,
    [Vector] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NULL,
    CONSTRAINT [PK_Embeddings] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Industries] (
    [IndustryId] int NOT NULL IDENTITY,
    [IndustryName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Industries] PRIMARY KEY ([IndustryId])
);
GO

CREATE TABLE [JobTypes] (
    [JobTypeId] int NOT NULL IDENTITY,
    [JobTypeName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_JobTypes] PRIMARY KEY ([JobTypeId])
);
GO

CREATE TABLE [Levels] (
    [LevelId] int NOT NULL IDENTITY,
    [LevelName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Levels] PRIMARY KEY ([LevelId])
);
GO

CREATE TABLE [Roles] (
    [RoleId] int NOT NULL IDENTITY,
    [RoleName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([RoleId])
);
GO

CREATE TABLE [SubscriptionTypes] (
    [SubscriptionTypeId] int NOT NULL IDENTITY,
    [PackageType] int NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [TryMatchLimit] int NOT NULL,
    [DurationInDays] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SubscriptionTypes] PRIMARY KEY ([SubscriptionTypeId])
);
GO

CREATE TABLE [Users] (
    [UserId] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NULL,
    [Email] nvarchar(450) NULL,
    [Image] nvarchar(max) NULL,
    [Phone] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
    [IsActive] bit NOT NULL,
    [RoleId] int NULL,
    [IsEmailVerified] bit NOT NULL,
    [EmailVerificationCode] nvarchar(max) NOT NULL,
    [EmailVerificationCodeExpiry] datetime2 NULL,
    [FirebaseUid] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([RoleId])
);
GO

CREATE TABLE [CandidateProfiles] (
    [CandidateProfileId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Gender] nvarchar(max) NULL,
    [Dob] datetime2 NULL,
    [JobTitle] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [Province] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [PersonalLink] nvarchar(max) NULL,
    [VideoUrl] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [AboutMeDescription] nvarchar(max) NULL,
    [SkillsJson] nvarchar(max) NULL,
    [EducationsJson] nvarchar(max) NULL,
    [WorkExperiencesJson] nvarchar(max) NULL,
    [AwardsJson] nvarchar(max) NULL,
    [HighlightProjectsJson] nvarchar(max) NULL,
    [CertificatesJson] nvarchar(max) NULL,
    [ForeignLanguagesJson] nvarchar(max) NULL,
    CONSTRAINT [PK_CandidateProfiles] PRIMARY KEY ([CandidateProfileId]),
    CONSTRAINT [FK_CandidateProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [CandidateSubscriptions] (
    [CandidateSubscriptionId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [SubscriptionTypeId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [RemainingTryMatches] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CandidateSubscriptions] PRIMARY KEY ([CandidateSubscriptionId]),
    CONSTRAINT [FK_CandidateSubscriptions_SubscriptionTypes_SubscriptionTypeId] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [SubscriptionTypes] ([SubscriptionTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CandidateSubscriptions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [CandidateToCompanyRequests] (
    [CandidateToCompanyRequestId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [CompanyProfileDescription] nvarchar(max) NULL,
    [Location] nvarchar(max) NULL,
    [TeamSize] nvarchar(max) NULL,
    [Website] nvarchar(max) NULL,
    [Contact] nvarchar(max) NULL,
    [IndustryId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CandidateToCompanyRequests] PRIMARY KEY ([CandidateToCompanyRequestId]),
    CONSTRAINT [FK_CandidateToCompanyRequests_Industries_IndustryId] FOREIGN KEY ([IndustryId]) REFERENCES [Industries] ([IndustryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CandidateToCompanyRequests_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [CompanyProfile] (
    [CompanyProfileId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [CompanyProfileDescription] nvarchar(max) NULL,
    [Location] nvarchar(max) NULL,
    [UrlCompanyLogo] nvarchar(max) NULL,
    [ImageLogoLgr] nvarchar(max) NULL,
    [TeamSize] nvarchar(max) NULL,
    [IsVerified] bit NOT NULL,
    [Website] nvarchar(max) NULL,
    [Contact] nvarchar(max) NULL,
    [IndustryId] int NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CompanyProfile] PRIMARY KEY ([CompanyProfileId]),
    CONSTRAINT [FK_CompanyProfile_Industries_IndustryId] FOREIGN KEY ([IndustryId]) REFERENCES [Industries] ([IndustryId]),
    CONSTRAINT [FK_CompanyProfile_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [CompanySubscriptions] (
    [CompanySubscriptionId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CompanySubscriptionTypeId] int NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [EndDate] datetime2 NOT NULL,
    [IsActive] bit NOT NULL,
    [RemainingJobPosts] int NOT NULL,
    [RemainingTrendingJobPosts] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CompanySubscriptions] PRIMARY KEY ([CompanySubscriptionId]),
    CONSTRAINT [FK_CompanySubscriptions_CompanySubscriptionTypes_CompanySubscriptionTypeId] FOREIGN KEY ([CompanySubscriptionTypeId]) REFERENCES [CompanySubscriptionTypes] ([CompanySubscriptionTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanySubscriptions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [CVs] (
    [CVId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [FileUrl] nvarchar(max) NOT NULL,
    [FullCvJson] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Type] int NOT NULL,
    CONSTRAINT [PK_CVs] PRIMARY KEY ([CVId]),
    CONSTRAINT [FK_CVs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Experiences] (
    [ExperienceId] int NOT NULL IDENTITY,
    [ExperienceName] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Experiences] PRIMARY KEY ([ExperienceId]),
    CONSTRAINT [FK_Experiences_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Jobs] (
    [JobId] int NOT NULL IDENTITY,
    [Title] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Education] nvarchar(max) NOT NULL,
    [YourSkill] nvarchar(max) NOT NULL,
    [YourExperience] nvarchar(max) NOT NULL,
    [CompanyId] int NOT NULL,
    [MinSalary] int NULL,
    [MaxSalary] int NULL,
    [IsSalaryNegotiable] bit NOT NULL,
    [IndustryId] int NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [LevelId] int NOT NULL,
    [JobTypeId] int NOT NULL,
    [Quantity] int NOT NULL,
    [TimeStart] datetime2 NOT NULL,
    [TimeEnd] datetime2 NOT NULL,
    [Status] int NOT NULL,
    [ProvinceName] nvarchar(max) NOT NULL,
    [AddressDetail] nvarchar(max) NOT NULL,
    [DeactivatedByAdmin] bit NOT NULL,
    [DescriptionWeight] real NOT NULL,
    [SkillsWeight] real NOT NULL,
    [ExperienceWeight] real NOT NULL,
    [EducationWeight] real NOT NULL,
    [IsTrending] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Jobs] PRIMARY KEY ([JobId]),
    CONSTRAINT [FK_Jobs_Industries_IndustryId] FOREIGN KEY ([IndustryId]) REFERENCES [Industries] ([IndustryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_JobTypes_JobTypeId] FOREIGN KEY ([JobTypeId]) REFERENCES [JobTypes] ([JobTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_Levels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [Levels] ([LevelId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_Users_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [Message] (
    [MessageId] int NOT NULL IDENTITY,
    [MessageText] nvarchar(max) NOT NULL,
    [FileUrl] nvarchar(max) NULL,
    [FileType] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [SenderId] int NOT NULL,
    [ReceiverId] int NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Message] PRIMARY KEY ([MessageId]),
    CONSTRAINT [FK_Message_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([UserId]),
    CONSTRAINT [FK_Message_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [Notifications] (
    [NotificationId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Title] nvarchar(max) NOT NULL,
    [Link] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [IsRead] bit NOT NULL,
    [JobTimeStart] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Notifications] PRIMARY KEY ([NotificationId]),
    CONSTRAINT [FK_Notifications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Payments] (
    [PaymentId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [SubscriptionTypeId] int NOT NULL,
    [TransactionCode] nvarchar(100) NOT NULL,
    [PaymentProvider] nvarchar(max) NOT NULL,
    [PaymentType] nvarchar(50) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [PaymentResponse] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([PaymentId]),
    CONSTRAINT [FK_Payments_SubscriptionTypes_SubscriptionTypeId] FOREIGN KEY ([SubscriptionTypeId]) REFERENCES [SubscriptionTypes] ([SubscriptionTypeId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Payments_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserFavoriteCompanies] (
    [UserId] int NOT NULL,
    [CompanyProfileId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserFavoriteCompanies] PRIMARY KEY ([UserId], [CompanyProfileId]),
    CONSTRAINT [FK_UserFavoriteCompanies_CompanyProfile_CompanyProfileId] FOREIGN KEY ([CompanyProfileId]) REFERENCES [CompanyProfile] ([CompanyProfileId]),
    CONSTRAINT [FK_UserFavoriteCompanies_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [Applications] (
    [ApplicationId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [JobId] int NOT NULL,
    [ResumeUrl] nvarchar(max) NULL,
    [CoverLetter] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [CvId] int NULL,
    [SimilarityScore] real NULL,
    [SimilarityDescription] real NULL,
    [SimilaritySkills] real NULL,
    [SimilarityExperience] real NULL,
    [SimilarityEducation] real NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY ([ApplicationId]),
    CONSTRAINT [FK_Applications_CVs_UniqueCvId] FOREIGN KEY ([CvId]) REFERENCES [CVs] ([CVId]),
    CONSTRAINT [FK_Applications_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]),
    CONSTRAINT [FK_Applications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [JobViews] (
    [JobViewId] int NOT NULL IDENTITY,
    [JobId] int NOT NULL,
    [UserId] int NULL,
    [IpAddress] nvarchar(max) NULL,
    [UserAgent] nvarchar(max) NULL,
    [ViewedAt] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_JobViews] PRIMARY KEY ([JobViewId]),
    CONSTRAINT [FK_JobViews_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]) ON DELETE CASCADE,
    CONSTRAINT [FK_JobViews_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [TryMatchRecords] (
    [TryMatchId] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [JobId] int NOT NULL,
    [CvId] int NULL,
    [SimilarityScore] real NULL,
    [Suggestions] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [Status] nvarchar(max) NULL,
    [ErrorMessage] nvarchar(max) NULL,
    CONSTRAINT [PK_TryMatchRecords] PRIMARY KEY ([TryMatchId]),
    CONSTRAINT [FK_TryMatchRecords_CVs_CvId] FOREIGN KEY ([CvId]) REFERENCES [CVs] ([CVId]),
    CONSTRAINT [FK_TryMatchRecords_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]),
    CONSTRAINT [FK_TryMatchRecords_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId])
);
GO

CREATE TABLE [UserFavoriteJobs] (
    [UserId] int NOT NULL,
    [JobId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserFavoriteJobs] PRIMARY KEY ([UserId], [JobId]),
    CONSTRAINT [FK_UserFavoriteJobs_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserFavoriteJobs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([UserId]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CompanySubscriptionTypeId', N'CreatedAt', N'CvMatchLimit', N'Description', N'DurationInDays', N'IsActive', N'JobPostLimit', N'Name', N'PackageType', N'Price', N'TrendingJobLimit', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[CompanySubscriptionTypes]'))
    SET IDENTITY_INSERT [CompanySubscriptionTypes] ON;
INSERT INTO [CompanySubscriptionTypes] ([CompanySubscriptionTypeId], [CreatedAt], [CvMatchLimit], [Description], [DurationInDays], [IsActive], [JobPostLimit], [Name], [PackageType], [Price], [TrendingJobLimit], [UpdatedAt])
VALUES (1, '2025-08-23T22:20:14.4595382', 5, N'Free tier with basic features', 30, CAST(1 AS bit), 2, N'Free', 0, 0.0, 0, '2025-08-23T22:20:14.4595403'),
(2, '2025-08-23T22:20:14.4595407', 10, N'Basic tier with extended features', 30, CAST(1 AS bit), 10, N'Basic', 1, 2000.0, 5, '2025-08-23T22:20:14.4595408'),
(3, '2025-08-23T22:20:14.4595412', 2147483647, N'Premium tier with unlimited features', 30, CAST(1 AS bit), 2147483647, N'Premium', 2, 3000.0, 10, '2025-08-23T22:20:14.4595414');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'CompanySubscriptionTypeId', N'CreatedAt', N'CvMatchLimit', N'Description', N'DurationInDays', N'IsActive', N'JobPostLimit', N'Name', N'PackageType', N'Price', N'TrendingJobLimit', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[CompanySubscriptionTypes]'))
    SET IDENTITY_INSERT [CompanySubscriptionTypes] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IndustryId', N'CreatedAt', N'IndustryName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Industries]'))
    SET IDENTITY_INSERT [Industries] ON;
INSERT INTO [Industries] ([IndustryId], [CreatedAt], [IndustryName], [UpdatedAt])
VALUES (3, '2025-08-23T22:20:14.4528072', N'Software Development', '2025-08-23T22:20:14.4528098'),
(4, '2025-08-23T22:20:14.4528101', N'Cybersecurity', '2025-08-23T22:20:14.4528102'),
(5, '2025-08-23T22:20:14.4528106', N'Data Science', '2025-08-23T22:20:14.4528107'),
(6, '2025-08-23T22:20:14.4528109', N'Cloud Computing', '2025-08-23T22:20:14.4528111'),
(7, '2025-08-23T22:20:14.4528113', N'UI/UX Design', '2025-08-23T22:20:14.4528115'),
(8, '2025-08-23T22:20:14.4528117', N'Artificial Intelligence', '2025-08-23T22:20:14.4528118'),
(9, '2025-08-23T22:20:14.4528121', N'DevOps', '2025-08-23T22:20:14.4528122');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IndustryId', N'CreatedAt', N'IndustryName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Industries]'))
    SET IDENTITY_INSERT [Industries] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'JobTypeId', N'CreatedAt', N'JobTypeName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[JobTypes]'))
    SET IDENTITY_INSERT [JobTypes] ON;
INSERT INTO [JobTypes] ([JobTypeId], [CreatedAt], [JobTypeName], [UpdatedAt])
VALUES (1, '2025-08-23T22:20:14.4528170', N'Full-time', '2025-08-23T22:20:14.4528171'),
(2, '2025-08-23T22:20:14.4528176', N'Part-time', '2025-08-23T22:20:14.4528177'),
(3, '2025-08-23T22:20:14.4528180', N'Remote', '2025-08-23T22:20:14.4528181');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'JobTypeId', N'CreatedAt', N'JobTypeName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[JobTypes]'))
    SET IDENTITY_INSERT [JobTypes] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'LevelId', N'CreatedAt', N'LevelName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Levels]'))
    SET IDENTITY_INSERT [Levels] ON;
INSERT INTO [Levels] ([LevelId], [CreatedAt], [LevelName], [UpdatedAt])
VALUES (1, '2025-08-23T22:20:14.4528225', N'Intern', '2025-08-23T22:20:14.4528226'),
(2, '2025-08-23T22:20:14.4528230', N'Fresher', '2025-08-23T22:20:14.4528231'),
(3, '2025-08-23T22:20:14.4528234', N'Junior', '2025-08-23T22:20:14.4528236'),
(4, '2025-08-23T22:20:14.4528238', N'Mid-level', '2025-08-23T22:20:14.4528240'),
(5, '2025-08-23T22:20:14.4528242', N'Senior', '2025-08-23T22:20:14.4528243'),
(6, '2025-08-23T22:20:14.4528246', N'Lead', '2025-08-23T22:20:14.4528248');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'LevelId', N'CreatedAt', N'LevelName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Levels]'))
    SET IDENTITY_INSERT [Levels] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedAt', N'RoleName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedAt], [RoleName], [UpdatedAt])
VALUES (1, '2025-08-23T15:20:14.4527823Z', N'Candidate', '2025-08-23T15:20:14.4527827Z'),
(2, '2025-08-23T15:20:14.4527833Z', N'Company', '2025-08-23T15:20:14.4527833Z'),
(3, '2025-08-23T15:20:14.4527834Z', N'Admin', '2025-08-23T15:20:14.4527834Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedAt', N'RoleName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SubscriptionTypeId', N'CreatedAt', N'Description', N'DurationInDays', N'IsActive', N'Name', N'PackageType', N'Price', N'TryMatchLimit', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[SubscriptionTypes]'))
    SET IDENTITY_INSERT [SubscriptionTypes] ON;
INSERT INTO [SubscriptionTypes] ([SubscriptionTypeId], [CreatedAt], [Description], [DurationInDays], [IsActive], [Name], [PackageType], [Price], [TryMatchLimit], [UpdatedAt])
VALUES (1, '2025-08-23T15:20:14.4595266Z', N'Free package with 1 try-match', 0, CAST(1 AS bit), N'Free', 1, 0.0, 1, '2025-08-23T15:20:14.4595270Z'),
(2, '2025-08-23T15:20:14.4595277Z', N'Basic package with 3 try-matches', 30, CAST(1 AS bit), N'Basic', 2, 2000.0, 3, '2025-08-23T15:20:14.4595278Z'),
(3, '2025-08-23T15:20:14.4595283Z', N'Premium package with 7 try-matches', 30, CAST(1 AS bit), N'Premium', 3, 3000.0, 7, '2025-08-23T15:20:14.4595284Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'SubscriptionTypeId', N'CreatedAt', N'Description', N'DurationInDays', N'IsActive', N'Name', N'PackageType', N'Price', N'TryMatchLimit', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[SubscriptionTypes]'))
    SET IDENTITY_INSERT [SubscriptionTypes] OFF;
GO

CREATE INDEX [IX_Applications_CvId] ON [Applications] ([CvId]);
GO

CREATE INDEX [IX_Applications_JobId] ON [Applications] ([JobId]);
GO

CREATE INDEX [IX_Applications_UserId] ON [Applications] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_CandidateProfiles_UserId] ON [CandidateProfiles] ([UserId]);
GO

CREATE INDEX [IX_CandidateSubscriptions_SubscriptionTypeId] ON [CandidateSubscriptions] ([SubscriptionTypeId]);
GO

CREATE INDEX [IX_CandidateSubscriptions_UserId] ON [CandidateSubscriptions] ([UserId]);
GO

CREATE INDEX [IX_CandidateToCompanyRequests_IndustryId] ON [CandidateToCompanyRequests] ([IndustryId]);
GO

CREATE UNIQUE INDEX [IX_CandidateToCompanyRequests_UserId] ON [CandidateToCompanyRequests] ([UserId]);
GO

CREATE INDEX [IX_CompanyProfile_IndustryId] ON [CompanyProfile] ([IndustryId]);
GO

CREATE UNIQUE INDEX [IX_CompanyProfile_UserId] ON [CompanyProfile] ([UserId]);
GO

CREATE INDEX [IX_CompanySubscriptions_CompanySubscriptionTypeId] ON [CompanySubscriptions] ([CompanySubscriptionTypeId]);
GO

CREATE INDEX [IX_CompanySubscriptions_UserId] ON [CompanySubscriptions] ([UserId]);
GO

CREATE INDEX [IX_CVs_UserId] ON [CVs] ([UserId]);
GO

CREATE INDEX [IX_Experiences_UserId] ON [Experiences] ([UserId]);
GO

CREATE INDEX [IX_Jobs_CompanyId] ON [Jobs] ([CompanyId]);
GO

CREATE INDEX [IX_Jobs_CreatedAt] ON [Jobs] ([CreatedAt]);
GO

CREATE INDEX [IX_Jobs_IndustryId] ON [Jobs] ([IndustryId]);
GO

CREATE INDEX [IX_Jobs_JobTypeId] ON [Jobs] ([JobTypeId]);
GO

CREATE INDEX [IX_Jobs_LevelId] ON [Jobs] ([LevelId]);
GO

CREATE INDEX [IX_JobViews_JobId] ON [JobViews] ([JobId]);
GO

CREATE INDEX [IX_JobViews_UserId] ON [JobViews] ([UserId]);
GO

CREATE INDEX [IX_Message_ReceiverId] ON [Message] ([ReceiverId]);
GO

CREATE INDEX [IX_Message_SenderId] ON [Message] ([SenderId]);
GO

CREATE INDEX [IX_Message_SentAt] ON [Message] ([SentAt]);
GO

CREATE INDEX [IX_Notifications_UserId] ON [Notifications] ([UserId]);
GO

CREATE INDEX [IX_Payments_SubscriptionTypeId] ON [Payments] ([SubscriptionTypeId]);
GO

CREATE INDEX [IX_Payments_UserId] ON [Payments] ([UserId]);
GO

CREATE INDEX [IX_TryMatchRecords_CvId] ON [TryMatchRecords] ([CvId]);
GO

CREATE INDEX [IX_TryMatchRecords_JobId] ON [TryMatchRecords] ([JobId]);
GO

CREATE INDEX [IX_TryMatchRecords_UserId] ON [TryMatchRecords] ([UserId]);
GO

CREATE INDEX [IX_UserFavoriteCompanies_CompanyProfileId] ON [UserFavoriteCompanies] ([CompanyProfileId]);
GO

CREATE INDEX [IX_UserFavoriteJobs_JobId] ON [UserFavoriteJobs] ([JobId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [Email] IS NOT NULL;
GO

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250823152016_initt', N'8.0.0');
GO

COMMIT;
GO

