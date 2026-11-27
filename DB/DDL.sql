CREATE DATABASE FPPR;
GO

USE FPPR;
GO

CREATE TABLE dbo.Users (
    UserID   NVARCHAR(20)  NOT NULL PRIMARY KEY,
    UserName NVARCHAR(50)  NOT NULL,
    Email    NVARCHAR(100) NOT NULL,
    Account  VARCHAR(20)   NOT NULL
);

CREATE TABLE dbo.Product (
    No          INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    ProductName NVARCHAR(100)     NOT NULL,
    Price       DECIMAL(18,2)     NOT NULL,
    FeeRate     DECIMAL(5,4)      NOT NULL
);

CREATE TABLE dbo.LikeList (
    SN               INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
    UserID           NVARCHAR(20)      NOT NULL REFERENCES dbo.Users(UserID),
    ProductNo        INT               NOT NULL REFERENCES dbo.Product(No),
    PurchaseQuantity INT               NOT NULL,
    Account          VARCHAR(20)       NOT NULL,
    TotalFee         DECIMAL(18,2)     NOT NULL,
    TotalAmount      DECIMAL(18,2)     NOT NULL
);
GO
