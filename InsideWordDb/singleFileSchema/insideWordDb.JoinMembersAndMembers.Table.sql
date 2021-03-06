USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[JoinMembersAndMembers]    Script Date: 06/04/2011 10:41:42 ******/
ALTER TABLE [insideWordDb].[JoinMembersAndMembers] DROP CONSTRAINT [FK_JoinMembersAndMembers_From_Members]
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers] DROP CONSTRAINT [FK_JoinMembersAndMembers_To_Members]
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers] DROP CONSTRAINT [FK_JoinMembersAndMembers_From_Members]
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers] DROP CONSTRAINT [FK_JoinMembersAndMembers_To_Members]
GO
DROP TABLE [insideWordDb].[JoinMembersAndMembers]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[JoinMembersAndMembers](
	[FromMemberId] [bigint] NOT NULL,
	[ToMemberId] [bigint] NOT NULL,
 CONSTRAINT [PK_JoinMembersAndAuthors] PRIMARY KEY CLUSTERED 
(
	[FromMemberId] ASC,
	[ToMemberId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers]  WITH CHECK ADD  CONSTRAINT [FK_JoinMembersAndMembers_From_Members] FOREIGN KEY([FromMemberId])
REFERENCES [insideWordDb].[Members] ([MemberId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers] CHECK CONSTRAINT [FK_JoinMembersAndMembers_From_Members]
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers]  WITH CHECK ADD  CONSTRAINT [FK_JoinMembersAndMembers_To_Members] FOREIGN KEY([ToMemberId])
REFERENCES [insideWordDb].[Members] ([MemberId])
GO
ALTER TABLE [insideWordDb].[JoinMembersAndMembers] CHECK CONSTRAINT [FK_JoinMembersAndMembers_To_Members]
GO
