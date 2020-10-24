using Dapper;
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
                    Id INT NOT NULL PRIMARY KEY,
                    Username VARCHAR(64) NOT NULL UNIQUE,
                    Password VARCHAR(64) NOT NULL,
                    Role VARCHAR(64) NOT NULL DEFAULT 'User'
                )
            ");

            conn.Execute($@"INSERT INTO [User] VALUES (1, 'Admin', 'lfdc82zo', 'Admin')");

            conn.Execute($@"
                CREATE TABLE [Instance] (
                    Id INT NOT NULL IDENTITY PRIMARY KEY,
                    UserId INT NOT NULL, 
                    Name VARCHAR(64) NOT NULL,
                    CONSTRAINT UN_Instance UNIQUE (UserId,Name),
                    CONSTRAINT FK_Instance_User FOREIGN KEY (UserId) REFERENCES [User] (Id)
                )
            ");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
