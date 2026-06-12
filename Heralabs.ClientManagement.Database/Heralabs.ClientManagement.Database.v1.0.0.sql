IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Client' and xtype='U')
BEGIN
	CREATE TABLE Client
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		ClientCode VARCHAR(50) NOT NULL UNIQUE,
		ClientName NVARCHAR(200) NOT NULL,
		SecurityCode VARCHAR(50) NOT NULL,
		ClientSecretKey VARCHAR(500) NOT NULL,
		Note NVARCHAR(MAX),
		Status INT NOT NULL, -- Active/Suspended/Expired
		SubscriptionStartDate DATETIME2,
		SubscriptionEndDate DATETIME2,
		Created DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
		CreatedBy VARCHAR(50) NOT NULL,
		Updated DATETIME2,
		UpdatedBy VARCHAR(50),
	);
	-- Indexes
	CREATE NONCLUSTERED INDEX idx_Client_ClientCode ON Client (ClientCode);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='WebApiVersion' and xtype='U')
BEGIN
	CREATE TABLE WebApiVersion
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		[Version] VARCHAR(10) NOT NULL,
		MinimumMobileAppVersion VARCHAR(10) NOT NULL,
		MaximumMobileAppVersion VARCHAR(10) NULL,
		ReleaseDate DATETIME2 NOT NULL,
		ReleaseNote NVARCHAR(MAX),
		IsActive BIT NOT NULL,
		Created DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
		CreatedBy VARCHAR(50) NOT NULL,
		Updated DATETIME2 NULL,
		UpdatedBy VARCHAR(50) NULL,
	);
	-- Indexes
	CREATE NONCLUSTERED INDEX idx_WebApiVersion_Version ON WebApiVersion ([Version]);
END
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='MobileAppVersion' and xtype='U')
BEGIN
	CREATE TABLE MobileAppVersion
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		AppPlatform VARCHAR(50) NOT NULL, --Android / IOS
		[Version] VARCHAR(10) NOT NULL,
		DownloadPackageUrl NVARCHAR(250),
		ReleaseDate DATETIME2 NOT NULL,
		ReleaseNote NVARCHAR(MAX),
		IsActive BIT NOT NULL,
		Created DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
		CreatedBy VARCHAR(50) NOT NULL,
		Updated DATETIME2 NULL,
		UpdatedBy VARCHAR(50) NULL,
	);
	-- Indexes
	CREATE NONCLUSTERED INDEX idx_MobileAppVersion_Version ON MobileAppVersion ([Version]);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='ClientDeployment' and xtype='U')
BEGIN
	CREATE TABLE ClientDeployment
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		ClientId UNIQUEIDENTIFIER NOT NULL,
		EnvironmentName NVARCHAR(50),

		ApiContainerName NVARCHAR(200),
		ApiContainerMappedPort INT,
		WebApiVersionCode VARCHAR(10),
		ApiUrl NVARCHAR(500),

		WebContainerName NVARCHAR(200),
		WebContainerMappedPort INT,
		WebVersionCode VARCHAR(10),
		WebUrl NVARCHAR(500),

		SqlContainerName NVARCHAR(200),
		SqlMappedPort INT,
		SqlHeralabsDbName VARCHAR(100),
		SqlHeralabsLogDbName VARCHAR(100),
		SqlHeralabsDbUser VARCHAR(50),
		SqlHeralabsDbPasswordEncrypted VARCHAR(500),

		PostgresContainerName NVARCHAR(200),
		PostgresMappedPort INT,
		PostgresHeralabsDbName VARCHAR(100),
		PostgresHeralabsDbUser VARCHAR(50),
		PostgresHeralabsDbPasswordEncrypted VARCHAR(500),

		IsActive BIT NOT NULL,
		Created DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
		CreatedBy VARCHAR(50) NOT NULL,
		Updated DATETIME2 NULL,
		UpdatedBy VARCHAR(50) NULL,
	);
	-- Indexes
	CREATE NONCLUSTERED INDEX idx_ClientDeployment_ClientId ON ClientDeployment (ClientId);
END
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='DeviceTracking' and xtype='U')
BEGIN
	CREATE TABLE DeviceTracking
	(
		[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
		ClientCode VARCHAR(50) NOT NULL,
		Username VARCHAR(50) NOT NULL,
		Os VARCHAR(50) NOT NULL,
		AppVersion VARCHAR(10),
		DeviceBrand NVARCHAR(250),
		DeviceModel NVARCHAR(250),
		DeviceId NVARCHAR(250),
		LoggedAt DATETIME2 NULL,
	);
	-- Indexes
	CREATE NONCLUSTERED INDEX idx_DeviceTracking_ClientCode ON DeviceTracking (ClientCode);
END
GO