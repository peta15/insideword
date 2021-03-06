USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[JoinArticlesAndPhotos]    Script Date: 06/04/2011 10:41:42 ******/
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos] DROP CONSTRAINT [FK_JoinPhotosAndArticles_Articles]
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos] DROP CONSTRAINT [FK_JoinPhotosAndArticles_Photos]
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos] DROP CONSTRAINT [FK_JoinPhotosAndArticles_Articles]
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos] DROP CONSTRAINT [FK_JoinPhotosAndArticles_Photos]
GO
DROP TABLE [insideWordDb].[JoinArticlesAndPhotos]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[JoinArticlesAndPhotos](
	[PhotoId] [bigint] NOT NULL,
	[ArticleId] [bigint] NOT NULL,
 CONSTRAINT [PK_JoinArticlesAndPhotos] PRIMARY KEY CLUSTERED 
(
	[PhotoId] ASC,
	[ArticleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos]  WITH CHECK ADD  CONSTRAINT [FK_JoinPhotosAndArticles_Articles] FOREIGN KEY([ArticleId])
REFERENCES [insideWordDb].[Articles] ([ArticleId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos] CHECK CONSTRAINT [FK_JoinPhotosAndArticles_Articles]
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos]  WITH CHECK ADD  CONSTRAINT [FK_JoinPhotosAndArticles_Photos] FOREIGN KEY([PhotoId])
REFERENCES [insideWordDb].[Photos] ([PhotoId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[JoinArticlesAndPhotos] CHECK CONSTRAINT [FK_JoinPhotosAndArticles_Photos]
GO
