using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Chargoon.ContainerManagement.Data.Migrations
{
    public class Migration_20201106_Command : IMigration
    {
        public void Up(SqlConnection conn)
        {
            conn.Execute($@"ALTER TABLE [Template] ADD [BeforeStartCommand] VARCHAR(256) NULL;");
            conn.Execute($@"ALTER TABLE [Template] ADD [AfterStartCommand] VARCHAR(256) NULL;");
            conn.Execute($@"ALTER TABLE [Template] ADD [BeforeStopCommand] VARCHAR(256) NULL;");
            conn.Execute($@"ALTER TABLE [Template] ADD [AfterStopCommand] VARCHAR(256) NULL;");
        }

        public void Down(SqlConnection conn)
        {

        }
    }
}
