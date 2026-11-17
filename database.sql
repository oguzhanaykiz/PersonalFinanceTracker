CREATE DATABASE PersonalFinanceDB;
GO

USE PersonalFinanceDB;
GO

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    FullName NVARCHAR(100) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categories (
    CategoryId INT PRIMARY KEY IDENTITY(1,1),
    CategoryName NVARCHAR(50) NOT NULL,
    CategoryType NVARCHAR(10) NOT NULL
);

CREATE TABLE Transactions (
    TransactionId INT PRIMARY KEY IDENTITY(1,1),
    UserId INT FOREIGN KEY REFERENCES Users(UserId),
    CategoryId INT FOREIGN KEY REFERENCES Categories(CategoryId),
    Amount DECIMAL(18, 2) NOT NULL,
    Description NVARCHAR(250),
    TransactionDate DATETIME DEFAULT GETDATE()
);

INSERT INTO Users (FullName) VALUES ('Oguzhan Aykiz');

INSERT INTO Categories (CategoryName, CategoryType) VALUES 
('Burs / Harclik', 'Gelir'),
('Freelance / Ek Gelir', 'Gelir'),
('Yemek & Kafe', 'Gider'),
('Ulasim', 'Gider'),
('Fatura & Abonelik', 'Gider'),
('Kitap & Egitim', 'Gider'),
('Diger', 'Gider');
