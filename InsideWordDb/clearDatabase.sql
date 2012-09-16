/*
Delete the tables in order of foreign keys to avoid leaving the database
in an unsafe state.
*/
use [insideWordDb]

TRUNCATE TABLE [insideWordDb].[JoinArticlesAndCategories]
GO

TRUNCATE TABLE [insideWordDb].[JoinArticlesAndMembers]
GO

TRUNCATE TABLE [insideWordDb].[JoinArticlesAndPhotos]
GO

TRUNCATE TABLE [insideWordDb].[JoinMembersAndMembers]
GO

TRUNCATE TABLE [insideWordDb].[JoinMembersAndPhotos]
GO

TRUNCATE TABLE [insideWordDb].[JoinMembershipsAndRoles]
GO

TRUNCATE TABLE [insideWordDb].[JoinRolesAndPermissions]
GO

TRUNCATE TABLE [insideWordDb].[Comments]
GO

TRUNCATE TABLE [insideWordDb].[ConversationScores]
GO

TRUNCATE TABLE [insideWordDb].[ConversationVotes]
GO

DELETE FROM [insideWordDb].[Conversations]
GO

DBCC CHECKIDENT ( [insideWordDb.Conversations], RESEED, 1)
GO

TRUNCATE TABLE [insideWordDb].[PhotoScores]
GO

TRUNCATE TABLE [insideWordDb].[PhotoVotes]
GO

DELETE FROM [insideWordDb].[Photos]
GO

DBCC CHECKIDENT ( [insideWordDb.Photos], RESEED, 1)
GO

TRUNCATE TABLE [insideWordDb].[AlternateCategoryIds]
GO

TRUNCATE TABLE [insideWordDb].[ArticleTexts]
GO

TRUNCATE TABLE [insideWordDb].[ArticleScores]
GO

TRUNCATE TABLE [insideWordDb].[ArticleVotes]
GO

TRUNCATE TABLE [insideWordDb].[AlternateArticleIds]
GO

DELETE FROM [insideWordDb].[Articles]
GO

DBCC CHECKIDENT ( [insideWordDb.Articles], RESEED, 1)
GO

DELETE FROM [insideWordDb].[Categories]
GO

DBCC CHECKIDENT ( [insideWordDb.Categories], RESEED, 1)
GO

DELETE FROM [insideWordDb].[Permissions]
GO

DBCC CHECKIDENT ( [insideWordDb.Permissions], RESEED, 1)
GO

DELETE FROM [insideWordDb].[Roles]
GO

DBCC CHECKIDENT ( [insideWordDb.Roles], RESEED, 1)
GO

DELETE FROM [insideWordDb].[Memberships]
GO

DBCC CHECKIDENT ( [insideWordDb.Memberships], RESEED, 1)
GO

DELETE FROM [insideWordDb].[Groups]
GO

DBCC CHECKIDENT ( [insideWordDb.Groups], RESEED, 1)
GO

TRUNCATE TABLE [insideWordDb].[AlternateMemberIds]
GO

DELETE FROM [insideWordDb].[Members]
GO

DBCC CHECKIDENT ( [insideWordDb.Members], RESEED, 1)
GO

TRUNCATE TABLE [insideWordDb].[InsideWordSettings]
GO