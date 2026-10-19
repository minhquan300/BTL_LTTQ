/* Apply manually to an existing database to enable card payments. */
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF EXISTS
(
    SELECT 1
    FROM sys.check_constraints
    WHERE name = N'CK_Payments_Method'
      AND parent_object_id = OBJECT_ID(N'dbo.Payments')
)
    ALTER TABLE dbo.Payments DROP CONSTRAINT CK_Payments_Method;

ALTER TABLE dbo.Payments WITH NOCHECK
    ADD CONSTRAINT CK_Payments_Method
    CHECK ([PaymentMethod] IN (N'Chuyen khoan', N'Tien mat', N'The'));

ALTER TABLE dbo.Payments CHECK CONSTRAINT CK_Payments_Method;

COMMIT TRANSACTION;
GO
