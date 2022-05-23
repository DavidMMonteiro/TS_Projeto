
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/23/2022 20:21:29
-- Generated from EDMX file: C:\Users\david\OneDrive - IPLeiria\Documents\GitHub\TS_Projeto\TS_Projeto_Chat\Server\ChatBD.edmx
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


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'UsersSet'
CREATE TABLE [dbo].[UsersSet] (
    [IdUser] int IDENTITY(1,1) NOT NULL,
    [Username] nvarchar(50)  NOT NULL,
    [SaltedPasswordHash] varbinary(32)  NOT NULL,
    [Salt] varbinary(9)  NOT NULL,
    [dtCreation] datetime  NOT NULL
);
GO

-- Creating table 'ChatsSet'
CREATE TABLE [dbo].[ChatsSet] (
    [IdChat] int IDENTITY(1,1) NOT NULL,
    [dtCreation] datetime  NOT NULL,
    [Name] nvarchar(50)  NOT NULL
);
GO

-- Creating table 'MensagensSet'
CREATE TABLE [dbo].[MensagensSet] (
    [IdMensagem] int IDENTITY(1,1) NOT NULL,
    [dtCreation] datetime  NOT NULL,
    [Text] nvarchar(max)  NOT NULL,
    [Chats_IdChat] int  NOT NULL,
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

-- Creating primary key on [IdChat] in table 'ChatsSet'
ALTER TABLE [dbo].[ChatsSet]
ADD CONSTRAINT [PK_ChatsSet]
    PRIMARY KEY CLUSTERED ([IdChat] ASC);
GO

-- Creating primary key on [IdMensagem] in table 'MensagensSet'
ALTER TABLE [dbo].[MensagensSet]
ADD CONSTRAINT [PK_MensagensSet]
    PRIMARY KEY CLUSTERED ([IdMensagem] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Chats_IdChat] in table 'MensagensSet'
ALTER TABLE [dbo].[MensagensSet]
ADD CONSTRAINT [FK_MensagensChats]
    FOREIGN KEY ([Chats_IdChat])
    REFERENCES [dbo].[ChatsSet]
        ([IdChat])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_MensagensChats'
CREATE INDEX [IX_FK_MensagensChats]
ON [dbo].[MensagensSet]
    ([Chats_IdChat]);
GO

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