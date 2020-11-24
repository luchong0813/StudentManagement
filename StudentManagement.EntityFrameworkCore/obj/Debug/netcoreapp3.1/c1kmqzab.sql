IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF SCHEMA_ID(N'School') IS NULL EXEC(N'CREATE SCHEMA [School];');

GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    [City] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Student] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(10) NOT NULL,
    [ClassName] int NOT NULL,
    [Email] nvarchar(max) NOT NULL,
    [Photo] nvarchar(max) NULL,
    [EnrollmentDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Student] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Teachers] (
    [Id] int NOT NULL IDENTITY,
    [TeacherName] nvarchar(50) NOT NULL,
    [HireDate] datetime2 NOT NULL,
    CONSTRAINT [PK_Teachers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [TodoItems] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [IsComplete] bit NOT NULL,
    CONSTRAINT [PK_TodoItems] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Departments] (
    [DepartmentId] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NULL,
    [Budget] money NOT NULL,
    [StartDate] datetime2 NOT NULL,
    [TeacherId] int NULL,
    [RowVersion] rowversion NULL,
    CONSTRAINT [PK_Departments] PRIMARY KEY ([DepartmentId]),
    CONSTRAINT [FK_Departments_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [OfficeLocations] (
    [TeacherId] int NOT NULL,
    [Location] nvarchar(50) NULL,
    CONSTRAINT [PK_OfficeLocations] PRIMARY KEY ([TeacherId]),
    CONSTRAINT [FK_OfficeLocations_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [School].[Course] (
    [CourseId] int NOT NULL,
    [Title] nvarchar(max) NULL,
    [Credits] int NOT NULL,
    [DepartmentId] int NOT NULL,
    CONSTRAINT [PK_Course] PRIMARY KEY ([CourseId]),
    CONSTRAINT [FK_Course_Departments_DepartmentId] FOREIGN KEY ([DepartmentId]) REFERENCES [Departments] ([DepartmentId]) ON DELETE NO ACTION
);

GO

CREATE TABLE [CourseAssignments] (
    [TeacherId] int NOT NULL,
    [CourseId] int NOT NULL,
    CONSTRAINT [PK_CourseAssignments] PRIMARY KEY ([CourseId], [TeacherId]),
    CONSTRAINT [FK_CourseAssignments_Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [School].[Course] ([CourseId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_CourseAssignments_Teachers_TeacherId] FOREIGN KEY ([TeacherId]) REFERENCES [Teachers] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [School].[StudentCourse] (
    [StudentCourseId] int NOT NULL IDENTITY,
    [CourseId] int NOT NULL,
    [StudentId] int NOT NULL,
    [Grade] int NULL,
    CONSTRAINT [PK_StudentCourse] PRIMARY KEY ([StudentCourseId]),
    CONSTRAINT [FK_StudentCourse_Course_CourseId] FOREIGN KEY ([CourseId]) REFERENCES [School].[Course] ([CourseId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_StudentCourse_Student_StudentId] FOREIGN KEY ([StudentId]) REFERENCES [Student] ([Id]) ON DELETE NO ACTION
);

GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);

GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;

GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);

GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);

GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);

GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;

GO

CREATE INDEX [IX_CourseAssignments_TeacherId] ON [CourseAssignments] ([TeacherId]);

GO

CREATE INDEX [IX_Departments_TeacherId] ON [Departments] ([TeacherId]);

GO

CREATE INDEX [IX_Course_DepartmentId] ON [School].[Course] ([DepartmentId]);

GO

CREATE INDEX [IX_StudentCourse_CourseId] ON [School].[StudentCourse] ([CourseId]);

GO

CREATE INDEX [IX_StudentCourse_StudentId] ON [School].[StudentCourse] ([StudentId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201124115755_InitialCreateBase', N'3.1.0');

GO

