USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[AlternateArticleIds]    Script Date: 06/04/2011 10:41:42 ******/
ALTER TABLE [insideWordDb].[AlternateArticleIds] DROP CONSTRAINT [FK_AlternateArticleIds_Articles]
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds] DROP CONSTRAINT [FK_AlternateArticleIds_Members]
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds] DROP CONSTRAINT [FK_AlternateArticleIds_Articles]
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds] DROP CONSTRAINT [FK_AlternateArticleIds_Members]
GO
DROP TABLE [insideWordDb].[AlternateArticleIds]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[AlternateArticleIds](
	[AlternateArticleIdId] [bigint] IDENTITY(1,1) NOT NULL,
	[SystemCreateDate] [datetime2](7) NOT NULL,
	[SystemEditDate] [datetime2](7) NOT NULL,
	[ArticleId] [bigint] NOT NULL,
	[MemberId] [bigint] NOT NULL,
	[AlternateId] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_AlternateArticleId] PRIMARY KEY CLUSTERED 
(
	[AlternateArticleIdId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_AlternateArticleIds_MemberId_AlternateId] ON [insideWordDb].[AlternateArticleIds] 
(
	[MemberId] ASC,
	[AlternateId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds]  WITH CHECK ADD  CONSTRAINT [FK_AlternateArticleIds_Articles] FOREIGN KEY([ArticleId])
REFERENCES [insideWordDb].[Articles] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds] CHECK CONSTRAINT [FK_AlternateArticleIds_Articles]
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds]  WITH CHECK ADD  CONSTRAINT [FK_AlternateArticleIds_Members] FOREIGN KEY([MemberId])
REFERENCES [insideWordDb].[Members] ([MemberId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[AlternateArticleIds] CHECK CONSTRAINT [FK_AlternateArticleIds_Members]
GO
