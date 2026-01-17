-- Script para criar o banco de dados e tabela de exemplo
-- Execute este script no SQL Server Management Studio ou via sqlcmd

-- Criar banco de dados (se não existir)
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LabCoffe')
BEGIN
    CREATE DATABASE LabCoffe;
END
GO

USE LabCoffe;
GO

-- Criar tabela Coffees
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Coffees')
BEGIN
    CREATE TABLE Coffees (
        Id UNIQUEIDENTIFIER PRIMARY KEY NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Description NVARCHAR(1000) NOT NULL,
        Price DECIMAL(18,2) NOT NULL,
        Stock INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NULL
    );

    -- Criar índices para melhor performance
    CREATE INDEX IX_Coffees_Name ON Coffees(Name);
    CREATE INDEX IX_Coffees_IsActive ON Coffees(IsActive);
    CREATE INDEX IX_Coffees_CreatedAt ON Coffees(CreatedAt);
END
GO

-- Inserir dados de exemplo (opcional)
-- INSERT INTO Coffees (Id, Name, Description, Price, Stock, IsActive, CreatedAt)
-- VALUES 
--     (NEWID(), 'Espresso', 'Strong and concentrated coffee', 5.50, 100, 1, GETUTCDATE()),
--     (NEWID(), 'Cappuccino', 'Espresso with steamed milk and foam', 7.50, 80, 1, GETUTCDATE()),
--     (NEWID(), 'Latte', 'Espresso with steamed milk', 8.00, 90, 1, GETUTCDATE());
-- GO

PRINT 'Database and table created successfully!';
GO
