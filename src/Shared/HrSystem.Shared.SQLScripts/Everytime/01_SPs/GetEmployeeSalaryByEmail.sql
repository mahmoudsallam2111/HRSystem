GO
/****** Object:  StoredProcedure [Hr].[GetEmployeeSalaryByEmail]    Script Date: 02/05/2025 1:03:44 PM ******/
SET ANSI_NULLS ON   -- Controls how NULL comparisons behave.
GO
SET QUOTED_IDENTIFIER ON    -- SELECT "FirstName" FROM Employees; -- Treated as column
GO
CREATE OR Alter PROCEDURE [Hr].[GetEmployeeSalaryByEmail]
    @Email NVARCHAR(256)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT Salary
    FROM [HRSystem].[Hr].[Employees]
    WHERE Email = @Email;
END;
Go