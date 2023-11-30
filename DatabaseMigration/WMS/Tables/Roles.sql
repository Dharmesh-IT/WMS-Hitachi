﻿CREATE TABLE [WMS].[Roles]
(
	[Id]  INT IDENTITY (1, 1) PRIMARY KEY, 
    [RoleName] NVARCHAR(MAX) NULL, 
    [IsActive] BIT NULL DEFAULT 1, 
)