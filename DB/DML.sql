USE FPPR;
GO

INSERT INTO dbo.Users (UserID, UserName, Email, Account) VALUES
(N'A1236456789', N'王o明', N'test@email.com', '1111999666'),
(N'B2345678901', N'陳o華', N'demo@email.com', '2222888555');

EXEC dbo.usp_LikeList_Insert N'A1236456789', N'0050',     150.00,  0.0100, 10, '1111999666';
EXEC dbo.usp_LikeList_Insert N'A1236456789', N'0056',   1000.00, 0.0150, 3,  '1111999666';
EXEC dbo.usp_LikeList_Insert N'B2345678901', N'00878', 500.00,  0.0200, 5,  '2222888555';
GO
