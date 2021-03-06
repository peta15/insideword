USE [insideWordDb]
GO
/****** Object:  Trigger [trg_CategoriesUpdate]    Script Date: 06/04/2011 10:41:43 ******/
DROP TRIGGER [insideWordDb].[trg_CategoriesUpdate]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TRIGGER [insideWordDb].[trg_CategoriesUpdate] ON [insideWordDb].[Categories] FOR UPDATE
AS
BEGIN
  IF @@ROWCOUNT = 0
        RETURN

    if UPDATE(ParentCategoryId)
    BEGIN
        UPDATE
            C
        SET
            HierarchyLevel    =
                C.HierarchyLevel - I.HierarchyLevel +
                    CASE
                        WHEN I.ParentCategoryId IS NULL THEN 0
                        ELSE Parent.HierarchyLevel + 1
                    END,
            FullPath =
                ISNULL(Parent.FullPath, '.') +
                CAST(I.CategoryId as varchar(10)) + '.' +
                RIGHT(C.FullPath, len(C.FullPath) - len(I.FullPath))
            FROM
                Categories AS C
            INNER JOIN
                inserted AS I ON C.FullPath LIKE I.FullPath + '%'
            LEFT OUTER JOIN
                Categories AS Parent ON I.ParentCategoryId = Parent.CategoryId
    END

END
GO
