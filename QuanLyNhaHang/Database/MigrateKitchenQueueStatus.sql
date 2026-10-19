/*
   Run once against the restaurant database before deploying the updated app.
   Adds the explicit kitchen queue state without changing existing order data.
*/
IF EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE name = N'CK_OrderDetails_Status' AND parent_object_id = OBJECT_ID(N'dbo.OrderDetails')
)
    ALTER TABLE dbo.OrderDetails DROP CONSTRAINT CK_OrderDetails_Status;
GO

ALTER TABLE dbo.OrderDetails WITH CHECK ADD CONSTRAINT CK_OrderDetails_Status
    CHECK (ItemStatus IN (N'Moi', N'Cho bep', N'Dang che bien', N'San sang', N'Da phuc vu', N'Da huy'));
GO
