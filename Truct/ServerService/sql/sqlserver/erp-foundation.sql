-- Apply after access-control.sql. Existing ERP tables are not changed by this script.
IF OBJECT_ID(N'dbo.erp_unit', N'U') IS NULL
CREATE TABLE dbo.erp_unit (
    Code nvarchar(20) NOT NULL PRIMARY KEY,
    Name nvarchar(150) NOT NULL,
    IsActive bit NOT NULL DEFAULT 1,
    SortOrder int NOT NULL DEFAULT 0
);

IF OBJECT_ID(N'dbo.erp_plant', N'U') IS NULL
CREATE TABLE dbo.erp_plant (
    Code nvarchar(20) NOT NULL PRIMARY KEY,
    UnitCode nvarchar(20) NOT NULL REFERENCES dbo.erp_unit(Code),
    Name nvarchar(150) NOT NULL,
    IsActive bit NOT NULL DEFAULT 1,
    SortOrder int NOT NULL DEFAULT 0
);

IF OBJECT_ID(N'dbo.erp_user_plant', N'U') IS NULL
CREATE TABLE dbo.erp_user_plant (
    UserId int NOT NULL,
    PlantCode nvarchar(20) NOT NULL REFERENCES dbo.erp_plant(Code),
    CONSTRAINT pk_erp_user_plant PRIMARY KEY (UserId, PlantCode)
);

IF OBJECT_ID(N'dbo.erp_notification', N'U') IS NULL
CREATE TABLE dbo.erp_notification (
    Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    Title nvarchar(200) NOT NULL,
    Body nvarchar(max) NOT NULL,
    UnitCode nvarchar(20) NULL,
    PlantCode nvarchar(20) NULL,
    RecipientUserId int NULL,
    CreatedAtUtc datetime2 NOT NULL,
    ExpiresAtUtc datetime2 NULL,
    CreatedByUserId int NOT NULL
);

IF OBJECT_ID(N'dbo.erp_notification_read', N'U') IS NULL
CREATE TABLE dbo.erp_notification_read (
    NotificationId bigint NOT NULL REFERENCES dbo.erp_notification(Id) ON DELETE CASCADE,
    UserId int NOT NULL,
    ReadAtUtc datetime2 NOT NULL,
    CONSTRAINT pk_erp_notification_read PRIMARY KEY (NotificationId, UserId)
);

IF OBJECT_ID(N'dbo.erp_setting', N'U') IS NULL
CREATE TABLE dbo.erp_setting (
    Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
    [Key] nvarchar(100) NOT NULL,
    [Value] nvarchar(max) NOT NULL,
    Scope nvarchar(50) NOT NULL,
    IsPublic bit NOT NULL DEFAULT 0,
    UpdatedAtUtc datetime2 NOT NULL,
    UpdatedByUserId int NOT NULL,
    CONSTRAINT ux_erp_setting_key_scope UNIQUE ([Key], Scope)
);
