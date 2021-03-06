USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[ConversationScores]    Script Date: 06/04/2011 10:41:42 ******/
ALTER TABLE [insideWordDb].[ConversationScores] DROP CONSTRAINT [FK_ConversationScores_Conversations]
GO
ALTER TABLE [insideWordDb].[ConversationScores] DROP CONSTRAINT [FK_ConversationScores_Conversations]
GO
DROP TABLE [insideWordDb].[ConversationScores]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[ConversationScores](
	[ConversationScoreId] [bigint] IDENTITY(1,1) NOT NULL,
	[SystemEditDate] [datetime2](7) NOT NULL,
	[SystemCreateDate] [datetime2](7) NOT NULL,
	[ConversationId] [bigint] NOT NULL,
	[Score] [float] NOT NULL,
 CONSTRAINT [PK_ConversationScores] PRIMARY KEY CLUSTERED 
(
	[ConversationScoreId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_ConversationId] ON [insideWordDb].[ConversationScores] 
(
	[ConversationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = ON, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[ConversationScores]  WITH CHECK ADD  CONSTRAINT [FK_ConversationScores_Conversations] FOREIGN KEY([ConversationId])
REFERENCES [insideWordDb].[Conversations] ([ConversationId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[ConversationScores] CHECK CONSTRAINT [FK_ConversationScores_Conversations]
GO
