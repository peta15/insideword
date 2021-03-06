USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[ArticleTexts]    Script Date: 06/04/2011 10:41:41 ******/
ALTER TABLE [insideWordDb].[ArticleTexts] DROP CONSTRAINT [FK_ArticleTexts_Articles]
GO
ALTER TABLE [insideWordDb].[ArticleTexts] DROP CONSTRAINT [FK_ArticleTexts_Articles]
GO
DROP TABLE [insideWordDb].[ArticleTexts]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[ArticleTexts](
	[ArticleTextId] [bigint] IDENTITY(1,1) NOT NULL,
	[ArticleId] [bigint] NOT NULL,
	[TextType] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ArticleText] PRIMARY KEY CLUSTERED 
(
	[ArticleTextId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[ArticleTexts]  WITH CHECK ADD  CONSTRAINT [FK_ArticleTexts_Articles] FOREIGN KEY([ArticleId])
REFERENCES [insideWordDb].[Articles] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[ArticleTexts] CHECK CONSTRAINT [FK_ArticleTexts_Articles]
GO
