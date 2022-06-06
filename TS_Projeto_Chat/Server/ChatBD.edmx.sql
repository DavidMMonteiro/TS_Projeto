
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/06/2022 18:24:52
-- Generated from EDMX file: C:\Users\2211849\Documents\GitHub\TS_Projeto\TS_Projeto_Chat\Server\ChatBD.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [ChatDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_MensagensUsers]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[MensagensSet] DROP CONSTRAINT [FK_MensagensUsers];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[UsersSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UsersSet];
GO
IF OBJECT_ID(N'[dbo].[MensagensSet]', 'U') IS NOT NULL
    DROP TABLE [dbo].[MensagensSet];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UsersSet'
CREATE TABLE [dbo].[UsersSet] (
    [IdUser] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [SaltedPasswordHash] varbinary(max)  NOT NULL,
    [Salt] varbinary(max)  NOT NULL,
    [dtCreation] datetime  NOT NULL
);
GO

-- Creating table 'MensagensSet'
CREATE TABLE [dbo].[MensagensSet] (
    [IdMensagem] int IDENTITY(1,1) NOT NULL,
    [dtCreation] datetime  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [Users_IdUser] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [IdUser] in table 'UsersSet'
ALTER TABLE [dbo].[UsersSet]
ADD CONSTRAINT [PK_UsersSet]
    PRIMARY KEY CLUSTERED ([IdUser] ASC);
GO

-- Creating primary key on [IdMensagem] in table 'MensagensSet'
ALTER TABLE [dbo].[MensagensSet]
ADD CONSTRAINT [PK_MensagensSet]
    PRIMARY KEY CLUSTERED ([IdMensagem] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Users_IdUser] in table 'MensagensSet'
ALTER TABLE [dbo].[MensagensSet]
ADD CONSTRAINT [FK_MensagensUsers]
    FOREIGN KEY ([Users_IdUser])
    REFERENCES [dbo].[UsersSet]
        ([IdUser])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MensagensUsers'
CREATE INDEX [IX_FK_MensagensUsers]
ON [dbo].[MensagensSet]
    ([Users_IdUser]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------