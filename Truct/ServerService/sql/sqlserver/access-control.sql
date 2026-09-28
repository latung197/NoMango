-- Apply to the same database as Core before deploying the access-control API.
-- Existing tables are left in place. Review their columns if they were created by an older app.

IF OBJECT_ID(N'dbo.sys_role', N'U') IS NULL
CREATE TABLE dbo.sys_role (
    role_id int IDENTITY(1,1) NOT NULL PRIMARY KEY,
    role_name nvarchar(100) NOT NULL,
    description nvarchar(max) NULL,
    validflg smallint NOT NULL DEFAULT 1,
    createtime datetime2 NULL, createid nvarchar(5) NULL,
    updatetime datetime2 NULL, updateid nvarchar(5) NULL,
    status nvarchar(1) NOT NULL DEFAULT N'1'
);

IF OBJECT_ID(N'dbo.sys_command', N'U') IS NULL
CREATE TABLE dbo.sys_command (
    menuid0 nvarchar(20) NOT NULL PRIMARY KEY,
    menuid nvarchar(20) NOT NULL DEFAULT N'',
    [text] nvarchar(100) NOT NULL DEFAULT N'',
    text2 nvarchar(100) NOT NULL DEFAULT N'',
    ma_ct nvarchar(3) NOT NULL DEFAULT N'',
    report nvarchar(100) NOT NULL DEFAULT N'',
    command nvarchar(100) NOT NULL DEFAULT N'',
    title nvarchar(max) NOT NULL DEFAULT N'',
    title2 nvarchar(max) NOT NULL DEFAULT N'',
    basicright smallint NOT NULL DEFAULT 0,
    picture1 nvarchar(max) NOT NULL DEFAULT N'',
    picture2 nvarchar(max) NOT NULL DEFAULT N'',
    [type] nvarchar(3) NOT NULL DEFAULT N'',
    sysid nvarchar(50) NOT NULL DEFAULT N'',
    syscode nvarchar(50) NOT NULL DEFAULT N'',
    hide_yn smallint NOT NULL DEFAULT 0,
    hide_yn2 smallint NULL,
    ds_dvcs nvarchar(max) NOT NULL DEFAULT N''
);

IF OBJECT_ID(N'dbo.sys_user_role', N'U') IS NULL
CREATE TABLE dbo.sys_user_role (
    user_id int NOT NULL,
    role_id int NOT NULL REFERENCES dbo.sys_role(role_id),
    createtime datetime2 NULL, createid nvarchar(5) NULL,
    updatetime datetime2 NULL, updateid nvarchar(5) NULL,
    status nvarchar(1) NOT NULL DEFAULT N'1',
    CONSTRAINT pk_sys_user_role PRIMARY KEY (user_id, role_id)
);

IF OBJECT_ID(N'dbo.sys_role_command', N'U') IS NULL
CREATE TABLE dbo.sys_role_command (
    role_id int NOT NULL REFERENCES dbo.sys_role(role_id),
    menuid0 nvarchar(20) NOT NULL REFERENCES dbo.sys_command(menuid0),
    can_view bit NOT NULL DEFAULT 0, can_add bit NOT NULL DEFAULT 0,
    can_edit bit NOT NULL DEFAULT 0, can_delete bit NOT NULL DEFAULT 0,
    can_print bit NOT NULL DEFAULT 0, can_import bit NOT NULL DEFAULT 0,
    can_export bit NOT NULL DEFAULT 0, can_search bit NOT NULL DEFAULT 0,
    can_reload bit NOT NULL DEFAULT 0, can_copy bit NOT NULL DEFAULT 0,
    can_approve bit NOT NULL DEFAULT 0,
    createtime datetime2 NULL, createid nvarchar(5) NULL,
    updatetime datetime2 NULL, updateid nvarchar(5) NULL,
    status nvarchar(1) NOT NULL DEFAULT N'1',
    CONSTRAINT pk_sys_role_command PRIMARY KEY (role_id, menuid0)
);

IF OBJECT_ID(N'dbo.sys_user_command', N'U') IS NULL
CREATE TABLE dbo.sys_user_command (
    user_id int NOT NULL,
    menuid0 nvarchar(20) NOT NULL REFERENCES dbo.sys_command(menuid0),
    can_view bit NOT NULL DEFAULT 0, can_add bit NOT NULL DEFAULT 0,
    can_edit bit NOT NULL DEFAULT 0, can_delete bit NOT NULL DEFAULT 0,
    can_print bit NOT NULL DEFAULT 0, can_import bit NOT NULL DEFAULT 0,
    can_export bit NOT NULL DEFAULT 0, can_search bit NOT NULL DEFAULT 0,
    can_reload bit NOT NULL DEFAULT 0, can_copy bit NOT NULL DEFAULT 0,
    can_approve bit NOT NULL DEFAULT 0,
    createtime datetime2 NULL, createid nvarchar(5) NULL,
    updatetime datetime2 NULL, updateid nvarchar(5) NULL,
    status nvarchar(1) NOT NULL DEFAULT N'1',
    CONSTRAINT pk_sys_user_command PRIMARY KEY (user_id, menuid0)
);
