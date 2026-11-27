USE FPPR;
GO
-- 取得所有使用者
CREATE PROCEDURE dbo.usp_User_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, UserName, Email, Account
    FROM dbo.Users
    ORDER BY UserID;
END
GO
-- 依使用者 ID 取得喜好金融商品清單
CREATE PROCEDURE dbo.usp_LikeList_GetByUser
    @UserID NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.SN, p.No AS ProductNo, p.ProductName, p.Price, p.FeeRate,
           l.PurchaseQuantity, l.Account, l.TotalFee, l.TotalAmount, u.Email
    FROM dbo.LikeList l
    JOIN dbo.Product p ON p.No = l.ProductNo
    JOIN dbo.Users u   ON u.UserID = l.UserID
    WHERE l.UserID = @UserID
    ORDER BY l.SN;
END
GO

-- 依流水號與使用者 ID 取得單筆喜好金融商品明細
CREATE PROCEDURE dbo.usp_LikeList_GetBySN
    @SN     INT,
    @UserID NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.SN, p.No AS ProductNo, p.ProductName, p.Price, p.FeeRate,
           l.PurchaseQuantity, l.Account, l.TotalFee, l.TotalAmount, u.Email
    FROM dbo.LikeList l
    JOIN dbo.Product p ON p.No = l.ProductNo
    JOIN dbo.Users u   ON u.UserID = l.UserID
    WHERE l.SN = @SN AND l.UserID = @UserID;
END
GO

-- 新增喜好金融商品
CREATE PROCEDURE dbo.usp_LikeList_Insert
    @UserID           NVARCHAR(20),
    @ProductName      NVARCHAR(100),
    @Price            DECIMAL(18,2),
    @FeeRate          DECIMAL(5,4),
    @PurchaseQuantity INT,
    @Account          VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON; --transaction

    DECLARE @TotalFee DECIMAL(18,2) = ROUND(@Price * @PurchaseQuantity * @FeeRate, 2);
    DECLARE @TotalAmount DECIMAL(18,2) = @Price * @PurchaseQuantity + @TotalFee;
    DECLARE @ProductNo INT, @SN INT;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO dbo.Product (ProductName, Price, FeeRate)
        VALUES (@ProductName, @Price, @FeeRate);
        SET @ProductNo = SCOPE_IDENTITY(); --取得剛剛 INSERT 進去的這筆資料所自動產生出來的流水號

        INSERT INTO dbo.LikeList (UserID, ProductNo, PurchaseQuantity, Account, TotalFee, TotalAmount)
        VALUES (@UserID, @ProductNo, @PurchaseQuantity, @Account, @TotalFee, @TotalAmount);
        SET @SN = SCOPE_IDENTITY();

        COMMIT TRANSACTION;
        SELECT @SN AS SN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION; --全數撤銷
        THROW;
    END CATCH
END
GO

-- 更新喜好金融商品
CREATE PROCEDURE dbo.usp_LikeList_Update
    @SN               INT,
    @UserID           NVARCHAR(20),
    @ProductName      NVARCHAR(100),
    @Price            DECIMAL(18,2),
    @FeeRate          DECIMAL(5,4),
    @PurchaseQuantity INT,
    @Account          VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @TotalFee DECIMAL(18,2) = ROUND(@Price * @PurchaseQuantity * @FeeRate, 2);
    DECLARE @TotalAmount DECIMAL(18,2) = @Price * @PurchaseQuantity + @TotalFee;
    DECLARE @ProductNo INT;

    SELECT @ProductNo = ProductNo FROM dbo.LikeList WHERE SN = @SN AND UserID = @UserID;
    IF @ProductNo IS NULL
    BEGIN
        SELECT 0 AS AffectedRows;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        UPDATE dbo.Product
        SET ProductName = @ProductName, Price = @Price, FeeRate = @FeeRate
        WHERE No = @ProductNo;

        UPDATE dbo.LikeList
        SET PurchaseQuantity = @PurchaseQuantity, Account = @Account,
            TotalFee = @TotalFee, TotalAmount = @TotalAmount
        WHERE SN = @SN;

        COMMIT TRANSACTION;
        SELECT 1 AS AffectedRows;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- 刪除喜好金融商品
CREATE PROCEDURE dbo.usp_LikeList_Delete
    @SN     INT,
    @UserID NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @ProductNo INT;

    SELECT @ProductNo = ProductNo FROM dbo.LikeList WHERE SN = @SN AND UserID = @UserID;
    IF @ProductNo IS NULL
    BEGIN
        SELECT 0 AS AffectedRows;
        RETURN;
    END

    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE FROM dbo.LikeList WHERE SN = @SN;
        DELETE FROM dbo.Product  WHERE No = @ProductNo;

        COMMIT TRANSACTION;
        SELECT 1 AS AffectedRows;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
