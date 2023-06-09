﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migration_20201014_Initial : IMigration
    {
        public void Up(SqlConnection conn)
        {
            conn.Execute($@"
                CREATE TABLE [User] (
                    Id INT NOT NULL IDENTITY PRIMARY KEY,
                    Username VARCHAR(64) NOT NULL UNIQUE,
                    Password VARCHAR(64) NOT NULL,
                    Roles VARCHAR(64) NOT NULL DEFAULT 'User'
                )
            ");

            conn.Execute($@"INSERT INTO [User] (Username,Password,Roles) VALUES ('Admin', 'lfdc82zo', 'Admin')");

            conn.Execute($@"
                CREATE TABLE [Template] (
                    Id INT NOT NULL IDENTITY PRIMARY KEY,
                    Name VARCHAR(64) NOT NULL,
                    DockerCompose NVARCHAR(MAX) NULL,
                    Environments NVARCHAR(MAX) NULL,
                    InsertCron VARCHAR(32) NULL,
                    IsActive BIT NOT NULL DEFAULT 1
                )
            ");

            conn.Execute($@"
                CREATE TABLE [Instance] (
                    Id INT NOT NULL IDENTITY PRIMARY KEY,
                    Code TINYINT NOT NULL,
                    UserId INT NOT NULL, 
                    TemplateId INT NULL,
                    Name VARCHAR(64) NOT NULL,
                    Environments NVARCHAR(MAX) NULL,
                    CONSTRAINT UN_Instance_Code UNIQUE (UserId,Code),
                    CONSTRAINT UN_Instance_Name UNIQUE (UserId,Name),
                    CONSTRAINT FK_Instance_User FOREIGN KEY (UserId) REFERENCES [User] (Id),
                    CONSTRAINT FK_Instance_Template FOREIGN KEY (TemplateId) REFERENCES [Template] (Id)
                )
            ");

            conn.Execute($@"
                CREATE TABLE [TemplateCommand] (
                    Id INT NOT NULL IDENTITY PRIMARY KEY,
                    TemplateId INT NOT NULL,
                    Name VARCHAR(64) NOT NULL,
                    ServiceName VARCHAR(64) NOT NULL,
                    Command NVARCHAR(MAX) NOT NULL,
                    Color TINYINT NOT NULL DEFAULT 0,
                    RunOnStartup BIT NOT NULL DEFAULT 0,
                    CONSTRAINT FK_TemplateCommand_Template FOREIGN KEY (TemplateId) REFERENCES [Template] (Id)
                )
            ");

            conn.Execute($@"
                CREATE TABLE [Image] (
                    Id INT NOT NULL IDENTITY PRIMARY KEY,
                    Name VARCHAR(64) NOT NULL,
                    BuildPath NVARCHAR(512) NULL,
                    BuildCron VARCHAR(32) NULL
                )
            ");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
