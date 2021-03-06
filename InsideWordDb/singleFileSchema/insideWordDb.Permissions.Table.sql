USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[Permissions]    Script Date: 06/04/2011 10:41:43 ******/
ALTER TABLE [insideWordDb].[Permissions] DROP CONSTRAINT [FK_Permissions_Groups]
GO
ALTER TABLE [insideWordDb].[Permissions] DROP CONSTRAINT [FK_Permissions_Groups]
GO
DROP TABLE [insideWordDb].[Permissions]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[Permissions](
	[PermissionId] [bigint] IDENTITY(1,1) NOT NULL,
	[SystemCreateDate] [datetime2](7) NOT NULL,
	[SystemEditDate] [datetime2](7) NOT NULL,
	[GroupId] [bigint] NULL,
	[ObjectTypeId] [bigint] NOT NULL,
	[Name] [nvarchar](32) NOT NULL,
	[Description] [nvarchar](256) NULL,
	[CanCreate] [bit] NOT NULL,
	[CanEdit] [bit] NOT NULL,
	[CanRead] [bit] NOT NULL,
	[CanDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Rights] PRIMARY KEY CLUSTERED 
(
	[PermissionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[Permissions]  WITH CHECK ADD  CONSTRAINT [FK_Permissions_Groups] FOREIGN KEY([GroupId])
REFERENCES [insideWordDb].[Groups] ([GroupId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[Permissions] CHECK CONSTRAINT [FK_Permissions_Groups]
GO
