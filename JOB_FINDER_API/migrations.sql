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

CREATE TABLE [CandidateToCompanyRequests] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CompanyName] nvarchar(max) NOT NULL,
    [CompanyProfileDescription] nvarchar(max) NULL,
    [Location] nvarchar(max) NULL,
    [TeamSize] nvarchar(max) NULL,
    [Website] nvarchar(max) NULL,
    [Contact] nvarchar(max) NULL,
    [IndustryId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CandidateToCompanyRequests] PRIMARY KEY ([Id])
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

CREATE TABLE [ExperienceLevel] (
    [id] int NOT NULL IDENTITY,
    [name] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_ExperienceLevel] PRIMARY KEY ([id])
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
    [Id] int NOT NULL IDENTITY,
    [JobTypeName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_JobTypes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Levels] (
    [Id] int NOT NULL IDENTITY,
    [LevelName] nvarchar(max) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Levels] PRIMARY KEY ([Id])
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

CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
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
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
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
    CONSTRAINT [PK_CandidateProfiles] PRIMARY KEY ([CandidateProfileId]),
    CONSTRAINT [FK_CandidateProfiles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CompanyProfile] (
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
    CONSTRAINT [PK_CompanyProfile] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_CompanyProfile_Industries_IndustryId] FOREIGN KEY ([IndustryId]) REFERENCES [Industries] ([IndustryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyProfile_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [Contacts] (
    [Id] nvarchar(450) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [UserId] int NOT NULL,
    [FacebookUrl] nvarchar(max) NULL,
    [LinkedInUrl] nvarchar(max) NULL,
    [GitHubUrl] nvarchar(max) NULL,
    [WebsiteUrl] nvarchar(max) NULL,
    CONSTRAINT [PK_Contacts] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Contacts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CVs] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [FileUrl] nvarchar(max) NOT NULL,
    [FullCvJson] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CVs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CVs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Experiences] (
    [Id] int NOT NULL IDENTITY,
    [ExperienceName] nvarchar(max) NOT NULL,
    [UserId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Experiences] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Experiences_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
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
    [ExperienceLevelId] int NOT NULL,
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
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Jobs] PRIMARY KEY ([JobId]),
    CONSTRAINT [FK_Jobs_ExperienceLevel_ExperienceLevelId] FOREIGN KEY ([ExperienceLevelId]) REFERENCES [ExperienceLevel] ([id]),
    CONSTRAINT [FK_Jobs_Industries_IndustryId] FOREIGN KEY ([IndustryId]) REFERENCES [Industries] ([IndustryId]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_JobTypes_JobTypeId] FOREIGN KEY ([JobTypeId]) REFERENCES [JobTypes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_Levels_LevelId] FOREIGN KEY ([LevelId]) REFERENCES [Levels] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Jobs_Users_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [Messages] (
    [Id] int NOT NULL IDENTITY,
    [MessageText] nvarchar(max) NOT NULL,
    [FileUrl] nvarchar(max) NULL,
    [FileType] nvarchar(max) NULL,
    [FileName] nvarchar(max) NULL,
    [SenderId] int NOT NULL,
    [ReceiverId] int NOT NULL,
    [SentAt] datetime2 NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Messages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Messages_Users_ReceiverId] FOREIGN KEY ([ReceiverId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Messages_Users_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [UserFavoriteCompanies] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [CompanyId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserFavoriteCompanies] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserFavoriteCompanies_Users_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_UserFavoriteCompanies_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [AboutMes] (
    [AboutMeId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [AboutMeDescription] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_AboutMes] PRIMARY KEY ([AboutMeId]),
    CONSTRAINT [FK_AboutMes_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Awards] (
    [AwardId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [AwardName] nvarchar(max) NULL,
    [AwardOrganization] nvarchar(max) NULL,
    [Month] datetime2 NULL,
    [Year] datetime2 NULL,
    [AwardDescription] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Awards] PRIMARY KEY ([AwardId]),
    CONSTRAINT [FK_Awards_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Certificates] (
    [CertificateId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [CertificateName] nvarchar(max) NOT NULL,
    [Organization] nvarchar(max) NULL,
    [Month] datetime2 NULL,
    [Year] datetime2 NULL,
    [CertificateUrl] nvarchar(max) NULL,
    [CertificateDescription] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Certificates] PRIMARY KEY ([CertificateId]),
    CONSTRAINT [FK_Certificates_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Educations] (
    [EducationId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [CreatedAt] datetime2 NULL,
    [UpdatedAt] datetime2 NULL,
    [School] nvarchar(max) NULL,
    [Degree] nvarchar(max) NULL,
    [Major] nvarchar(max) NULL,
    [IsStudying] bit NULL,
    [MonthStart] datetime2 NULL,
    [YearStart] datetime2 NULL,
    [MonthEnd] datetime2 NULL,
    [YearEnd] datetime2 NULL,
    [Detail] nvarchar(max) NULL,
    CONSTRAINT [PK_Educations] PRIMARY KEY ([EducationId]),
    CONSTRAINT [FK_Educations_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [ForeignLanguages] (
    [ForeignLanguageId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [LanguageName] nvarchar(max) NULL,
    [LanguageLevel] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ForeignLanguages] PRIMARY KEY ([ForeignLanguageId]),
    CONSTRAINT [FK_ForeignLanguages_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [HighlightProjects] (
    [HighlightProjectId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [ProjectName] nvarchar(max) NULL,
    [IsWorking] bit NOT NULL,
    [MonthStart] datetime2 NULL,
    [YearStart] datetime2 NULL,
    [MonthEnd] datetime2 NULL,
    [YearEnd] datetime2 NULL,
    [ProjectDescription] nvarchar(max) NULL,
    [Technologies] nvarchar(max) NULL,
    [Responsibilities] nvarchar(max) NULL,
    [TeamSize] nvarchar(max) NULL,
    [Achievements] nvarchar(max) NULL,
    [ProjectLink] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_HighlightProjects] PRIMARY KEY ([HighlightProjectId]),
    CONSTRAINT [FK_HighlightProjects_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Skills] (
    [SkillId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NULL,
    [GroupName] nvarchar(max) NULL,
    [SkillName] nvarchar(max) NOT NULL,
    [Experience] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Skills] PRIMARY KEY ([SkillId]),
    CONSTRAINT [FK_Skills_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [WorkExperiences] (
    [WorkExperienceId] int NOT NULL IDENTITY,
    [CandidateProfileId] int NOT NULL,
    [JobTitle] nvarchar(max) NULL,
    [CompanyName] nvarchar(max) NULL,
    [IsWorking] bit NOT NULL,
    [MonthStart] datetime2 NULL,
    [YearStart] datetime2 NULL,
    [MonthEnd] datetime2 NULL,
    [YearEnd] datetime2 NULL,
    [WorkDescription] nvarchar(max) NULL,
    [Responsibilities] nvarchar(max) NULL,
    [Achievements] nvarchar(max) NULL,
    [Technologies] nvarchar(max) NULL,
    [ProjectName] nvarchar(max) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_WorkExperiences] PRIMARY KEY ([WorkExperienceId]),
    CONSTRAINT [FK_WorkExperiences_CandidateProfiles_CandidateProfileId] FOREIGN KEY ([CandidateProfileId]) REFERENCES [CandidateProfiles] ([CandidateProfileId]) ON DELETE CASCADE
);
GO

CREATE TABLE [Applications] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [JobId] int NOT NULL,
    [ResumeUrl] nvarchar(max) NULL,
    [CoverLetter] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [SubmittedAt] datetime2 NOT NULL,
    [CvId] int NOT NULL,
    [SimilarityScore] real NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Applications] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Applications_CVs_UniqueCvId] FOREIGN KEY ([CvId]) REFERENCES [CVs] ([Id]),
    CONSTRAINT [FK_Applications_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]),
    CONSTRAINT [FK_Applications_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [UserFavoriteJobs] (
    [UserId] int NOT NULL,
    [JobId] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [PK_UserFavoriteJobs] PRIMARY KEY ([UserId], [JobId]),
    CONSTRAINT [FK_UserFavoriteJobs_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]),
    CONSTRAINT [FK_UserFavoriteJobs_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
);
GO

CREATE TABLE [JobSkills] (
    [JobId] int NOT NULL,
    [SkillId] int NOT NULL,
    CONSTRAINT [PK_JobSkills] PRIMARY KEY ([JobId], [SkillId]),
    CONSTRAINT [FK_JobSkills_Jobs_JobId] FOREIGN KEY ([JobId]) REFERENCES [Jobs] ([JobId]),
    CONSTRAINT [FK_JobSkills_Skills_SkillId] FOREIGN KEY ([SkillId]) REFERENCES [Skills] ([SkillId])
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'name') AND [object_id] = OBJECT_ID(N'[ExperienceLevel]'))
    SET IDENTITY_INSERT [ExperienceLevel] ON;
INSERT INTO [ExperienceLevel] ([id], [name])
VALUES (1, N'Fresher'),
(2, N'Junior'),
(3, N'Middle'),
(4, N'Senior');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'id', N'name') AND [object_id] = OBJECT_ID(N'[ExperienceLevel]'))
    SET IDENTITY_INSERT [ExperienceLevel] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IndustryId', N'CreatedAt', N'IndustryName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Industries]'))
    SET IDENTITY_INSERT [Industries] ON;
INSERT INTO [Industries] ([IndustryId], [CreatedAt], [IndustryName], [UpdatedAt])
VALUES (3, '2025-06-29T15:02:27.5769456Z', N'Software Development', '2025-06-29T15:02:27.5769456Z'),
(4, '2025-06-29T15:02:27.5769459Z', N'Cybersecurity', '2025-06-29T15:02:27.5769459Z'),
(5, '2025-06-29T15:02:27.5769460Z', N'Data Science', '2025-06-29T15:02:27.5769461Z'),
(6, '2025-06-29T15:02:27.5769462Z', N'Cloud Computing', '2025-06-29T15:02:27.5769462Z'),
(7, '2025-06-29T15:02:27.5769464Z', N'UI/UX Design', '2025-06-29T15:02:27.5769464Z'),
(8, '2025-06-29T15:02:27.5769465Z', N'Artificial Intelligence', '2025-06-29T15:02:27.5769466Z'),
(9, '2025-06-29T15:02:27.5769467Z', N'DevOps', '2025-06-29T15:02:27.5769467Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'IndustryId', N'CreatedAt', N'IndustryName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Industries]'))
    SET IDENTITY_INSERT [Industries] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'JobTypeName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[JobTypes]'))
    SET IDENTITY_INSERT [JobTypes] ON;
INSERT INTO [JobTypes] ([Id], [CreatedAt], [JobTypeName], [UpdatedAt])
VALUES (1, '2025-06-29T15:02:27.5769510Z', N'Full-time', '2025-06-29T15:02:27.5769511Z'),
(2, '2025-06-29T15:02:27.5769513Z', N'Part-time', '2025-06-29T15:02:27.5769514Z'),
(3, '2025-06-29T15:02:27.5769515Z', N'Remote', '2025-06-29T15:02:27.5769516Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'JobTypeName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[JobTypes]'))
    SET IDENTITY_INSERT [JobTypes] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'LevelName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Levels]'))
    SET IDENTITY_INSERT [Levels] ON;
INSERT INTO [Levels] ([Id], [CreatedAt], [LevelName], [UpdatedAt])
VALUES (1, '2025-06-29T15:02:27.5769607Z', N'Intern', '2025-06-29T15:02:27.5769607Z'),
(2, '2025-06-29T15:02:27.5769609Z', N'Junior', '2025-06-29T15:02:27.5769609Z'),
(3, '2025-06-29T15:02:27.5769611Z', N'Senior', '2025-06-29T15:02:27.5769611Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedAt', N'LevelName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Levels]'))
    SET IDENTITY_INSERT [Levels] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedAt', N'RoleName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] ON;
INSERT INTO [Roles] ([RoleId], [CreatedAt], [RoleName], [UpdatedAt])
VALUES (1, '2025-06-29T15:02:27.5769267Z', N'Candidate', '2025-06-29T15:02:27.5769270Z'),
(2, '2025-06-29T15:02:27.5769275Z', N'Company', '2025-06-29T15:02:27.5769276Z'),
(3, '2025-06-29T15:02:27.5769277Z', N'Admin', '2025-06-29T15:02:27.5769277Z');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'RoleId', N'CreatedAt', N'RoleName', N'UpdatedAt') AND [object_id] = OBJECT_ID(N'[Roles]'))
    SET IDENTITY_INSERT [Roles] OFF;
GO

CREATE INDEX [IX_AboutMes_CandidateProfileId] ON [AboutMes] ([CandidateProfileId]);
GO

CREATE INDEX [IX_Applications_CvId] ON [Applications] ([CvId]);
GO

CREATE INDEX [IX_Applications_JobId] ON [Applications] ([JobId]);
GO

CREATE INDEX [IX_Applications_UserId] ON [Applications] ([UserId]);
GO

CREATE INDEX [IX_Awards_CandidateProfileId] ON [Awards] ([CandidateProfileId]);
GO

CREATE UNIQUE INDEX [IX_CandidateProfiles_UserId] ON [CandidateProfiles] ([UserId]);
GO

CREATE INDEX [IX_Certificates_CandidateProfileId] ON [Certificates] ([CandidateProfileId]);
GO

CREATE INDEX [IX_CompanyProfile_IndustryId] ON [CompanyProfile] ([IndustryId]);
GO

CREATE INDEX [IX_Contacts_UserId] ON [Contacts] ([UserId]);
GO

CREATE INDEX [IX_CVs_UserId] ON [CVs] ([UserId]);
GO

CREATE INDEX [IX_Educations_CandidateProfileId] ON [Educations] ([CandidateProfileId]);
GO

CREATE INDEX [IX_Experiences_UserId] ON [Experiences] ([UserId]);
GO

CREATE INDEX [IX_ForeignLanguages_CandidateProfileId] ON [ForeignLanguages] ([CandidateProfileId]);
GO

CREATE INDEX [IX_HighlightProjects_CandidateProfileId] ON [HighlightProjects] ([CandidateProfileId]);
GO

CREATE INDEX [IX_Jobs_CompanyId] ON [Jobs] ([CompanyId]);
GO

CREATE INDEX [IX_Jobs_CreatedAt] ON [Jobs] ([CreatedAt]);
GO

CREATE INDEX [IX_Jobs_ExperienceLevelId] ON [Jobs] ([ExperienceLevelId]);
GO

CREATE INDEX [IX_Jobs_IndustryId] ON [Jobs] ([IndustryId]);
GO

CREATE INDEX [IX_Jobs_JobTypeId] ON [Jobs] ([JobTypeId]);
GO

CREATE INDEX [IX_Jobs_LevelId] ON [Jobs] ([LevelId]);
GO

CREATE INDEX [IX_JobSkills_SkillId] ON [JobSkills] ([SkillId]);
GO

CREATE INDEX [IX_Messages_ReceiverId] ON [Messages] ([ReceiverId]);
GO

CREATE INDEX [IX_Messages_SenderId] ON [Messages] ([SenderId]);
GO

CREATE INDEX [IX_Messages_SentAt] ON [Messages] ([SentAt]);
GO

CREATE INDEX [IX_Skills_CandidateProfileId] ON [Skills] ([CandidateProfileId]);
GO

CREATE INDEX [IX_UserFavoriteCompanies_CompanyId] ON [UserFavoriteCompanies] ([CompanyId]);
GO

CREATE INDEX [IX_UserFavoriteCompanies_UserId] ON [UserFavoriteCompanies] ([UserId]);
GO

CREATE INDEX [IX_UserFavoriteJobs_JobId] ON [UserFavoriteJobs] ([JobId]);
GO

CREATE UNIQUE INDEX [IX_Users_Email] ON [Users] ([Email]) WHERE [Email] IS NOT NULL;
GO

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
GO

CREATE INDEX [IX_WorkExperiences_CandidateProfileId] ON [WorkExperiences] ([CandidateProfileId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250629150228_init', N'8.0.0');
GO

COMMIT;
GO

