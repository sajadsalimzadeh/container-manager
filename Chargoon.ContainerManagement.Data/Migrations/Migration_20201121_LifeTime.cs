using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migration_20201121_LifeTime : IMigration
    {
        public void Up(SqlConnection conn)
        {
            conn.Execute($@"ALTER TABLE [Template] ADD [InsertLifeTime] INT NULL;");
            conn.Execute($@"ALTER TABLE [Template] ADD [ExpireTime] DATETIME NULL;");
            conn.Execute($@"ALTER TABLE [Image] ADD [LifeTime] INT NULL;");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
