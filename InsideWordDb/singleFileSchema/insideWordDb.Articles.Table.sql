USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[Articles]    Script Date: 06/04/2011 10:41:41 ******/
ALTER TABLE [insideWordDb].[Articles] DROP CONSTRAINT [FK_Articles_Members]
GO
ALTER TABLE [insideWordDb].[Articles] DROP CONSTRAINT [FK_Articles_Members]
GO
DROP TABLE [insideWordDb].[Articles]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[Articles](
	[ArticleId] [bigint] IDENTITY(1,1) NOT NULL,
	[SystemEditDate] [datetime2](7) NOT NULL,
	[SystemCreateDate] [datetime2](7) NOT NULL,
	[EditDate] [datetime2](7) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Title] [nvarchar](128) NOT NULL,
	[MemberId] [bigint] NULL,
	[Blurb] [nvarchar](256) NULL,
	[IsHidden] [bit] NOT NULL,
	[ReadCount] [int] NOT NULL,
	[IgnoreFlags] [bit] NOT NULL,
	[IsPublished] [bit] NOT NULL,
	[BlurbIsAutoGen] [bit] NOT NULL,
 CONSTRAINT [PK_articles] PRIMARY KEY CLUSTERED 
(
	[ArticleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[Articles]  WITH CHECK ADD  CONSTRAINT [FK_Articles_Members] FOREIGN KEY([MemberId])
REFERENCES [insideWordDb].[Members] ([MemberId])
ON DELETE SET NULL
GO
ALTER TABLE [insideWordDb].[Articles] CHECK CONSTRAINT [FK_Articles_Members]
GO
