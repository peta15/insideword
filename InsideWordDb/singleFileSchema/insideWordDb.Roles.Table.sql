USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[Roles]    Script Date: 06/04/2011 10:41:43 ******/
ALTER TABLE [insideWordDb].[Roles] DROP CONSTRAINT [FK_Roles_Groups]
GO
ALTER TABLE [insideWordDb].[Roles] DROP CONSTRAINT [FK_Roles_Groups]
GO
DROP TABLE [insideWordDb].[Roles]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[Roles](
	[RoleId] [bigint] IDENTITY(1,1) NOT NULL,
	[SystemCreateDate] [datetime2](7) NOT NULL,
	[SystemEditDate] [datetime2](7) NOT NULL,
	[GroupId] [bigint] NULL,
	[Name] [nvarchar](32) NOT NULL,
	[Description] [nvarchar](256) NULL,
 CONSTRAINT [PK_Roles] PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[Roles]  WITH CHECK ADD  CONSTRAINT [FK_Roles_Groups] FOREIGN KEY([GroupId])
REFERENCES [insideWordDb].[Groups] ([GroupId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[Roles] CHECK CONSTRAINT [FK_Roles_Groups]
GO
