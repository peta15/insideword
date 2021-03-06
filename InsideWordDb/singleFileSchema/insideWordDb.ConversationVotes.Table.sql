USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[ConversationVotes]    Script Date: 06/04/2011 10:41:42 ******/
ALTER TABLE [insideWordDb].[ConversationVotes] DROP CONSTRAINT [FK_ConversationVotes_Conversations]
GO
ALTER TABLE [insideWordDb].[ConversationVotes] DROP CONSTRAINT [FK_ConversationVotes_Members]
GO
ALTER TABLE [insideWordDb].[ConversationVotes] DROP CONSTRAINT [FK_ConversationVotes_Conversations]
GO
ALTER TABLE [insideWordDb].[ConversationVotes] DROP CONSTRAINT [FK_ConversationVotes_Members]
GO
DROP TABLE [insideWordDb].[ConversationVotes]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[ConversationVotes](
	[ConversationVoteId] [bigint] IDENTITY(1,1) NOT NULL,
	[SystemEditDate] [datetime2](7) NOT NULL,
	[SystemCreateDate] [datetime2](7) NOT NULL,
	[ConversationId] [bigint] NOT NULL,
	[MemberId] [bigint] NULL,
	[VoteWeight] [int] NOT NULL,
	[IsShadowVote] [bit] NOT NULL,
	[IsFlag] [bit] NOT NULL,
	[Text] [nvarchar](512) NOT NULL,
	[IsHidden] [bit] NOT NULL,
 CONSTRAINT [PK_CommentVote] PRIMARY KEY CLUSTERED 
(
	[ConversationVoteId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX [FK_ConversationId] ON [insideWordDb].[ConversationVotes] 
(
	[ConversationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[ConversationVotes]  WITH CHECK ADD  CONSTRAINT [FK_ConversationVotes_Conversations] FOREIGN KEY([ConversationId])
REFERENCES [insideWordDb].[Conversations] ([ConversationId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[ConversationVotes] CHECK CONSTRAINT [FK_ConversationVotes_Conversations]
GO
ALTER TABLE [insideWordDb].[ConversationVotes]  WITH CHECK ADD  CONSTRAINT [FK_ConversationVotes_Members] FOREIGN KEY([MemberId])
REFERENCES [insideWordDb].[Members] ([MemberId])
ON DELETE SET NULL
GO
ALTER TABLE [insideWordDb].[ConversationVotes] CHECK CONSTRAINT [FK_ConversationVotes_Members]
GO
