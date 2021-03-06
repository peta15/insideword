USE [insideWordDb]
GO
/****** Object:  Table [insideWordDb].[JoinMembershipsAndRoles]    Script Date: 06/04/2011 10:41:43 ******/
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles] DROP CONSTRAINT [FK_JoinMembershipsAndRoles_Memberships]
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles] DROP CONSTRAINT [FK_JoinMembershipsAndRoles_Roles]
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles] DROP CONSTRAINT [FK_JoinMembershipsAndRoles_Memberships]
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles] DROP CONSTRAINT [FK_JoinMembershipsAndRoles_Roles]
GO
DROP TABLE [insideWordDb].[JoinMembershipsAndRoles]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [insideWordDb].[JoinMembershipsAndRoles](
	[MembershipId] [bigint] NOT NULL,
	[RoleId] [bigint] NOT NULL,
 CONSTRAINT [PK_JoinMembershipsAndRoles] PRIMARY KEY CLUSTERED 
(
	[MembershipId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles]  WITH CHECK ADD  CONSTRAINT [FK_JoinMembershipsAndRoles_Memberships] FOREIGN KEY([MembershipId])
REFERENCES [insideWordDb].[Memberships] ([MembershipId])
ON DELETE CASCADE
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles] CHECK CONSTRAINT [FK_JoinMembershipsAndRoles_Memberships]
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles]  WITH CHECK ADD  CONSTRAINT [FK_JoinMembershipsAndRoles_Roles] FOREIGN KEY([RoleId])
REFERENCES [insideWordDb].[Roles] ([RoleId])
GO
ALTER TABLE [insideWordDb].[JoinMembershipsAndRoles] CHECK CONSTRAINT [FK_JoinMembershipsAndRoles_Roles]
GO
